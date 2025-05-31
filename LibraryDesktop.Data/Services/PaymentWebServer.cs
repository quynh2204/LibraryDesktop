using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using LibraryDesktop.Data.Interfaces;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryDesktop.Data.Services
{    public class PaymentWebServer
    {
        private WebServer? _server;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _webRootPath;

        public PaymentWebServer(IServiceProvider serviceProvider, string webRootPath)
        {
            _serviceProvider = serviceProvider;
            _webRootPath = webRootPath;
        }public async Task StartAsync(int port = 5000)
        {
            try
            {
                Console.WriteLine($"🚀 Starting PaymentWebServer on port {port}...");
                
                // Get local IPv4 address automatically
                var localIP = GetLocalIPAddress();
                
                // Create server - bind to localhost first, then try + for network access
                _server = CreateWebServer($"http://+:{port}/");
                
                Console.WriteLine($"✅ PaymentWebServer created, starting on http://+:{port}/");
                Console.WriteLine($"🌐 Access payment pages at: http://localhost:{port}/payment?token=TEST");
                if (!string.IsNullOrEmpty(localIP))
                {
                    Console.WriteLine($"🌐 Or from other devices at: http://{localIP}:{port}/payment?token=TEST");
                }
                
                await _server.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ PaymentWebServer failed to start: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                throw;
            }
        }        private WebServer CreateWebServer(string url)
        {
            try
            {
                Console.WriteLine($"🔧 Creating WebServer with URL: {url}");
                Console.WriteLine($"🔧 WebRoot path: {_webRootPath}");
                Console.WriteLine($"🔧 WebRoot exists: {Directory.Exists(_webRootPath)}");
                  var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                    .WithAction("/api/payment", HttpVerbs.Get, HandlePaymentDetailsApi)
                    .WithAction("/api/payment", HttpVerbs.Options, HandleCorsPreflightRequest)
                    .WithAction("/confirm-payment", HttpVerbs.Post, HandlePaymentConfirmation)
                    .WithAction("/confirm-payment", HttpVerbs.Options, HandleCorsPreflightRequest)
                    .WithAction("/payment", HttpVerbs.Get, HandlePaymentPage)
                    .WithAction("/api/register", HttpVerbs.Post, HandleRegistration)
                    .WithStaticFolder("/", _webRootPath, true);

                Console.WriteLine($"✅ WebServer created successfully");
                return server;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to create WebServer: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task HandleCorsPreflightRequest(IHttpContext context)
        {
            AddCorsHeaders(context);
            context.Response.StatusCode = 200;
            await Task.CompletedTask;
        }

        private void AddCorsHeaders(IHttpContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
            context.Response.Headers.Add("Access-Control-Max-Age", "86400");
        }        private async Task HandlePaymentPage(IHttpContext context)
        {
            try
            {
                Console.WriteLine($"🔍 HandlePaymentPage called");
                
                var data = context.Request.QueryString["data"];
                var token = context.Request.QueryString["token"]; // Fallback for old format

                Console.WriteLine($"🔍 Data parameter: {data}");
                Console.WriteLine($"🔍 Token parameter: {token}");

                // Check if have either data or token
                if (string.IsNullOrEmpty(data) && string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"❌ Missing data or token parameter");
                    context.Response.StatusCode = 400;
                    var errorBytes = Encoding.UTF8.GetBytes("Missing data or token parameter");
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    return;
                }

                // Serve the static payment page - JavaScript will handle data extraction
                var indexPath = Path.Combine(_webRootPath, "index.html");
                Console.WriteLine($"🔍 Index path: {indexPath}");
                Console.WriteLine($"🔍 Index exists: {File.Exists(indexPath)}");
                
                if (File.Exists(indexPath))
                {
                    var content = await File.ReadAllTextAsync(indexPath);
                    
                    var responseBytes = Encoding.UTF8.GetBytes(content);
                    context.Response.ContentType = "text/html";
                    await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine($"✅ Payment page served successfully");
                }
                else
                {
                    Console.WriteLine($"❌ Payment page not found: {indexPath}");
                    context.Response.StatusCode = 500;
                    var errorBytes = Encoding.UTF8.GetBytes("Payment page not found");
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in HandlePaymentPage: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                
                context.Response.StatusCode = 500;
                var errorBytes = Encoding.UTF8.GetBytes($"Internal server error: {ex.Message}");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
            }
        }        private async Task HandlePaymentDetailsApi(IHttpContext context)
        {
            AddCorsHeaders(context);
            Console.WriteLine("➡️ Received GET /api/payment");
            try
            {
                Console.WriteLine("➡️ API/payment called");

                // Check for embedded data parameter first
                var dataParam = context.Request.QueryString["data"];                if (!string.IsNullOrEmpty(dataParam))
                {
                    Console.WriteLine("➡️ Found embedded data parameter");
                    // Decode embedded payment data
                    var paymentDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(dataParam));
                    var paymentData = JsonSerializer.Deserialize<JsonElement>(paymentDataJson);
                    
                    var userId = paymentData.GetProperty("userId").GetInt32();
                      // Get username using UserService
                    using var userScope = _serviceProvider.CreateScope();
                    var userService = userScope.ServiceProvider.GetService<IUserService>();
                    var username = userService != null
                        ? await userService.GetUsernameByIdAsync(userId) ?? $"User {userId}"
                        : $"User {userId}";
                      var amount = paymentData.GetProperty("amount").GetInt32();
                    var coins = amount / 1000;
                      var jsonResponse = JsonSerializer.Serialize(new
                    {
                        amount = amount.ToString(),
                        user = username,
                        description = "Account recharge",
                        status = "Pending",
                        created = DateTime.Now,
                        token = paymentData.GetProperty("token").GetString()
                    });

                    var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    return;
                }                // Fallback to token-based lookup - Create scoped service
                using var fallbackScope = _serviceProvider.CreateScope();
                var paymentService = fallbackScope.ServiceProvider.GetRequiredService<IPaymentService>();
                var fallbackUserService = fallbackScope.ServiceProvider.GetService<IUserService>();
                
                var token = context.Request.QueryString["token"];
                if (string.IsNullOrEmpty(token))
                    throw new Exception("Missing token or data parameter");

                var payment = await paymentService.GetPaymentByTokenAsync(token);
                if (payment == null)
                    throw new Exception("Payment not found");                // Get username from UserService
                var fallbackUsername = fallbackUserService != null 
                    ? await fallbackUserService.GetUsernameByIdAsync(payment.UserId) ?? $"User {payment.UserId}"
                    : $"User {payment.UserId}";var fallbackResponse = JsonSerializer.Serialize(new
                {
                    amount = payment.Amount.ToString(),
                    user = fallbackUsername,
                    description = payment.Description,
                    status = payment.PaymentStatus.ToString(),
                    created = payment.CreatedDate,
                });

                var fallbackBytes = Encoding.UTF8.GetBytes(fallbackResponse);
                context.Response.ContentType = "application/json";
                await context.Response.OutputStream.WriteAsync(fallbackBytes, 0, fallbackBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error in /api/payment: " + ex.Message);
                context.Response.StatusCode = 500;
                await context.SendStringAsync("{\"error\": \"" + ex.Message + "\"}", "application/json", Encoding.UTF8);
            }
        }        private async Task HandlePaymentConfirmation(IHttpContext context)
        {
            AddCorsHeaders(context);

            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                var body = await reader.ReadToEndAsync();
                
                Console.WriteLine($"💳 Payment confirmation request body: {body}");
                
                // Parse confirmation data
                var confirmationData = JsonSerializer.Deserialize<JsonElement>(body);
                
                // Check if have embedded data or just a token
                bool hasEmbeddedData = confirmationData.TryGetProperty("paymentData", out var paymentDataElement);
                
                if (hasEmbeddedData)
                {
                    Console.WriteLine("💳 Processing payment with embedded data");
                    // Handle self-contained payment data
                    var success = await ProcessEmbeddedPaymentData(paymentDataElement);
                    
                    var response = new
                    {
                        success = success,
                        message = success ? "Payment completed successfully" : "Payment confirmation failed",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    var jsonResponse = JsonSerializer.Serialize(response);
                    var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
                else if (confirmationData.TryGetProperty("token", out var tokenElement))
                {
                    Console.WriteLine("💳 Processing payment with token (fallback)");
                    // Fallback to token-based payment completion - Create scoped service
                    using var scope = _serviceProvider.CreateScope();
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                    
                    var token = tokenElement.GetString();
                    if (string.IsNullOrEmpty(token))
                    {
                        context.Response.StatusCode = 400;
                        var errorBytes = Encoding.UTF8.GetBytes("{\"success\":false,\"message\":\"Invalid request - missing token\"}");
                        context.Response.ContentType = "application/json";
                        await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                        return;
                    }

                    var success = await paymentService.CompletePaymentAsync(token);

                    var response = new
                    {
                        success = success,
                        message = success ? "Payment completed successfully" : "Payment confirmation failed",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    var jsonResponse = JsonSerializer.Serialize(response);
                    var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    var errorBytes = Encoding.UTF8.GetBytes("{\"success\":false,\"message\":\"Invalid request - missing token or payment data\"}");
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in HandlePaymentConfirmation: {ex.Message}");
                context.Response.StatusCode = 500;
                var errorResponse = new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                };
                var errorBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(errorResponse));
                context.Response.ContentType = "application/json";
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
            }
        }        private async Task<bool> ProcessEmbeddedPaymentData(JsonElement paymentDataElement)
        {
            try
            {
                var token = paymentDataElement.GetProperty("token").GetString();
                var amount = paymentDataElement.GetProperty("amount").GetInt32();
                var userId = paymentDataElement.GetProperty("userId").GetInt32();
                var timestamp = paymentDataElement.GetProperty("timestamp").GetInt64();

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("❌ Token is null or empty");
                    return false;
                }

                Console.WriteLine($"💳 Processing embedded payment: Token={token}, Amount={amount}, UserId={userId}, Timestamp={timestamp}");

                // Validate timestamp (optional - reject payments older than 1 hour)
                var paymentTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                if (DateTimeOffset.Now.Subtract(paymentTime).TotalHours > 1)
                {
                    Console.WriteLine("❌ Payment expired (older than 1 hour)");
                    return false;
                }

                // Create and save payment record - Create scoped service
                using var scope = _serviceProvider.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                
                var success = await paymentService.CreateAndCompletePaymentAsync(userId, amount, token);

                Console.WriteLine($"💳 Payment processing result: {success}");
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing embedded payment data: {ex.Message}");
                return false;
            }
        }        private async Task HandleRegistration(IHttpContext context)
        {
            try
            {
                // Create scoped service for authentication
                using var scope = _serviceProvider.CreateScope();
                var authenticationService = scope.ServiceProvider.GetService<IAuthenticationService>();
                
                if (authenticationService == null)
                {
                    context.Response.StatusCode = 503;
                    var errorBytes = Encoding.UTF8.GetBytes("{\"error\":\"Registration service not available\"}");
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    return;
                }

                using var reader = new StreamReader(context.Request.InputStream);
                var body = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(body))
                {
                    context.Response.StatusCode = 400;
                    var errorBytes = Encoding.UTF8.GetBytes("{\"error\":\"Empty request body\"}");
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    return;
                }

                var registrationData = JsonSerializer.Deserialize<RegistrationData>(
                    body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (registrationData?.Username == null || registrationData.Email == null || registrationData.Password == null)
                {
                    context.Response.StatusCode = 400;
                    var errorBytes = Encoding.UTF8.GetBytes("{\"error\":\"Missing required fields\"}");
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    return;
                }

                var user = await authenticationService.RegisterAsync(registrationData.Username, registrationData.Email, registrationData.Password);

                if (user != null)
                {
                    var response = JsonSerializer.Serialize(new { success = true, message = "Registration successful", userId = user.UserId });
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
                else
                {
                    context.Response.StatusCode = 409;
                    var response = JsonSerializer.Serialize(new { success = false, message = "Username or email already exists" });
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                var errorBytes = Encoding.UTF8.GetBytes($"{{\"error\":\"Registration failed: {ex.Message}\"}}");
                context.Response.ContentType = "application/json";
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
            }
        }


        public async Task StopAsync()
        {
            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
            await Task.CompletedTask;
        }

        private class PaymentConfirmationData
        {
            public string? Token { get; set; }
        }        private class RegistrationData
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        private string GetLocalIPAddress()
        {
            try
            {
                using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    var endPoint = socket.LocalEndPoint as System.Net.IPEndPoint;
                    return endPoint?.Address.ToString() ?? "";
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
