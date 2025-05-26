using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Models;

namespace LibraryDesktop.Data.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserSettingRepository _userSettingRepository;

        public PaymentService(IPaymentRepository paymentRepository, IUserSettingRepository userSettingRepository)
        {
            _paymentRepository = paymentRepository;
            _userSettingRepository = userSettingRepository;
        }

        public async Task<Payment> CreatePaymentAsync(int userId, decimal amount, string description = "")
        {
            var payment = new Payment
            {
                UserId = userId,
                Amount = amount,
                PaymentMethod = PaymentMethod.QRCode,
                PaymentStatus = PaymentStatus.Pending,
                PaymentToken = GeneratePaymentToken(),
                CreatedDate = DateTime.Now,
                Description = description
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return payment;
        }        public async Task<string> GenerateQRCodeAsync(Payment payment)
        {
            // Create payment URL for Live Server with user information
            var paymentUrl = $"{NetworkConfiguration.GetLiveServerUrl()}?token={payment.PaymentToken}&amount={payment.Amount}&userId={payment.UserId}";
            
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
                }

                // Complete the payment
                await _paymentRepository.CompletePaymentAsync(token);

                // Update user balance (convert amount to coins: 1000 VND = 1 coin)
                var userSetting = await _userSettingRepository.GetByUserIdAsync(payment.UserId);
                if (userSetting != null)
                {
                    var coinsToAdd = payment.Amount / 1000; // 1000 VND = 1 coin
                    userSetting.Balance += coinsToAdd;
                    await _userSettingRepository.UpdateAsync(userSetting);
                    await _userSettingRepository.SaveChangesAsync();
                }

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
        }

        public async Task<Payment?> GetPaymentByTokenAsync(string token)
        {
            return await _paymentRepository.GetPaymentByTokenAsync(token);
        }

        private string GeneratePaymentToken()
        {
            return Guid.NewGuid().ToString("N")[..16]; // 16 character token
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
