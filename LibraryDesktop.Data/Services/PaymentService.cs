using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Models;
using System.Text.Json;
using System.Text;

namespace LibraryDesktop.Data.Services
{    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserSettingRepository _userSettingRepository;
        private readonly IUserRepository _userRepository;
        
        // Event to notify khi payment completed
        public event EventHandler<PaymentCompletedEventArgs>? PaymentCompleted;

        public PaymentService(IPaymentRepository paymentRepository, IUserSettingRepository userSettingRepository, IUserRepository userRepository)
        {
            _paymentRepository = paymentRepository;
            _userSettingRepository = userSettingRepository;
            _userRepository = userRepository;
        }        public async Task<Payment> CreatePaymentAsync(int userId, int amount, string description = "")
        {
            // Verify user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception($"User with ID {userId} not found");

            // Generate a unique payment token with retry logic
            string paymentToken;
            int retryCount = 0;
            const int maxRetries = 5;
            
            do
            {
                paymentToken = GeneratePaymentToken();
                var existingPayment = await _paymentRepository.GetPaymentByTokenAsync(paymentToken);
                if (existingPayment == null)
                    break;
                    
                retryCount++;
                if (retryCount >= maxRetries)
                    throw new Exception("Unable to generate unique payment token after multiple attempts");
                    
            } while (retryCount < maxRetries);

            var payment = new Payment
            {
                UserId = userId,
                Amount = amount,
                PaymentMethod = PaymentMethod.QRCode,
                PaymentStatus = PaymentStatus.Pending,
                PaymentToken = paymentToken,
                CreatedDate = DateTime.Now,
                Description = description
            };            try
            {
                await _paymentRepository.AddAsync(payment);
                await _paymentRepository.SaveChangesAsync();
                
                return payment;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Handle specific database constraint violations
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed: Payments.PaymentToken") == true)
                {
                    throw new Exception($"Payment token collision detected. Please try again. Token: {paymentToken}");
                }
                else if (dbEx.InnerException?.Message.Contains("FOREIGN KEY constraint failed") == true)
                {
                    throw new Exception($"Invalid user ID: {userId}. User not found in database.");
                }
                else
                {
                    throw new Exception($"Database error while creating payment: {dbEx.InnerException?.Message ?? dbEx.Message}", dbEx);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create payment: {ex.Message}", ex);
            }
        }        public async Task<string> GenerateQRCodeAsync(Payment payment)
        {            // Create payment URL for Live Server with user information
            var paymentUrl = $"{NetworkConfiguration.GetLiveServerUrl()}?token={payment.PaymentToken}&userId={payment.UserId}";
            
            // Update payment with URL
            payment.PaymentUrl = paymentUrl;
            
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            
            var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
            
            // Update payment with QR code data and URL
            payment.QrCodeData = qrCodeBase64;
            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            
            return qrCodeBase64;
        }        public async Task<bool> CompletePaymentAsync(string token)
        {
            try
            {
                // Get payment details first
                var payment = await _paymentRepository.GetPaymentByTokenAsync(token);
                if (payment == null || payment.PaymentStatus != PaymentStatus.Pending)
                {
                    return false;
                }                // Complete the payment (this also updates user balance)
                await _paymentRepository.CompletePaymentAsync(token);

                // 🔥 FIRE EVENT: Announce payment completed
                PaymentCompleted?.Invoke(this, new PaymentCompletedEventArgs
                {
                    UserId = payment.UserId,
                    Amount = payment.Amount,
                    PaymentToken = token,
                    CompletedAt = DateTime.Now
                });

                Console.WriteLine($"💰 Payment completed! User {payment.UserId} received {payment.Amount / 1000} coins");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Payment>> GetUserPaymentsAsync(int userId)
        {
            return await _paymentRepository.GetUserPaymentsAsync(userId);
        }        public async Task<Payment?> GetPaymentByTokenAsync(string token)
        {
            return await _paymentRepository.GetPaymentByTokenAsync(token);
        }

        /// <summary>
        /// Creates and immediately completes a payment (for self-contained QR codes)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="amount">Payment amount</param>
        /// <param name="token">Payment token</param>
        /// <param name="description">Payment description</param>
        /// <returns>True if successful</returns>
        public async Task<bool> CreateAndCompletePaymentAsync(int userId, int amount, string token, string description = "")
        {
            try
            {
                // Verify user exists
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    Console.WriteLine($"❌ User with ID {userId} not found");
                    return false;
                }

                // Check if payment with this token already exists (prevent double processing)
                var existingPayment = await _paymentRepository.GetPaymentByTokenAsync(token);                if (existingPayment != null)
                {
                    Console.WriteLine($"❌ Payment with token {token} already exists");
                    return false;
                }
                
                // Create payment record
                var payment = new Payment
                {
                    UserId = userId,
                    Amount = amount,
                    PaymentMethod = PaymentMethod.QRCode,
                    PaymentStatus = PaymentStatus.Pending, // Start as pending, will be completed by CompletePaymentAsync
                    PaymentToken = token,
                    CreatedDate = DateTime.Now,
                    Description = string.IsNullOrEmpty(description) ? "Account recharge" : description
                };
                
                // Save payment to database
                await _paymentRepository.AddAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                // Complete the payment (this will update user balance automatically)
                // Note: CompletePaymentAsync will fire the PaymentCompleted event, so we don't fire it here
                await _paymentRepository.CompletePaymentAsync(token);

                Console.WriteLine($"💰 Payment created and completed! User {userId} received {amount / 1000} coins (Token: {token})");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in CreateAndCompletePaymentAsync: {ex.Message}");
                return false;
            }
        }private string GeneratePaymentToken()
        {
            // Create a highly unique token using timestamp + GUID + random
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            var guid = Guid.NewGuid().ToString("N")[..6]; // 6 characters from GUID
            var random = new Random().Next(100, 999).ToString(); // 3 digit random number
            return $"{timestamp}{guid}{random}"[..20]; // Ensure max 20 characters
        }        /// <summary>
        /// Generates QR code directly without database operations
        /// </summary>
        /// <param name="paymentToken">Payment token</param>
        /// <param name="amount">Payment amount</param>
        /// <param name="userId">User ID (optional)</param>
        /// <returns>Base64 encoded QR code image</returns>        public string GenerateQRCodeForPayment(string paymentToken, int amount, int? userId = null)        /// <summary>
        /// Generates QR code directly without database operations
        /// </summary>
        /// <param name="paymentToken">Payment token</param>
        /// <param name="amount">Payment amount</param>
        /// <param name="userId">User ID (optional)</param>
        /// <returns>Base64 encoded QR code image</returns>
        public string GenerateQRCodeForPayment(string paymentToken, int amount, int? userId = null)
        {
            try
            {
                // Encode payment data into the token for self-contained QR code
                var paymentData = new
                {
                    token = paymentToken,
                    amount = amount,
                    userId = userId ?? 1,
                    timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
                };
                
                var paymentDataJson = JsonSerializer.Serialize(paymentData);
                var paymentDataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(paymentDataJson));
                
                // Create payment URL with embedded data
                var paymentUrl = $"{NetworkConfiguration.GetLiveServerUrl()}?data={paymentDataBase64}";
                
                Console.WriteLine($"🔗 Generated QR Payment URL: {paymentUrl}");
                Console.WriteLine($"🔗 Server Host: {NetworkConfiguration.LIVE_SERVER_HOST}");
                Console.WriteLine($"🔗 Network Info: {NetworkConfiguration.GetNetworkInfo()}");
                
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);
                
                var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
                return qrCodeBase64;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate QR code: {ex.Message}", ex);
            }
        }
    }

    public class GitHubContentService : IGitHubContentService
    {
        private readonly HttpClient _httpClient;

        public GitHubContentService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "LibraryApp-Reader/1.0");
        }

        public async Task<string> GetContentAsync(string url)
        {
            if (!IsValidGitHubRawUrl(url))
            {
                url = ConvertToRawUrl(url);
            }

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to fetch content from GitHub: {ex.Message}");
            }
        }

        public bool IsValidGitHubRawUrl(string url)
        {
            return url.Contains("raw.githubusercontent.com") || url.Contains("github.com") && url.Contains("/raw/");
        }

        public string ConvertToRawUrl(string githubUrl)
        {
            // Convert GitHub URL to raw content URL
            if (githubUrl.Contains("github.com") && githubUrl.Contains("/blob/"))
            {
                return githubUrl.Replace("github.com", "raw.githubusercontent.com").Replace("/blob/", "/");
            }
            return githubUrl;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
