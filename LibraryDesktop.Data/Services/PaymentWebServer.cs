using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using LibraryDesktop.Data.Interfaces;
using System.Text.Json;
using System.Text;

namespace LibraryDesktop.Data.Services
{    public class PaymentWebServer
    {
        private WebServer? _server;
        private readonly IPaymentService _paymentService;
        private readonly IAuthenticationService _authenticationService;
        private readonly string _webRootPath;

        public PaymentWebServer(IPaymentService paymentService, string webRootPath, IAuthenticationService? authenticationService = null)
        {
            _paymentService = paymentService;
            _webRootPath = webRootPath;
            _authenticationService = authenticationService!;
        }        public async Task StartAsync(int port = 5000)
        {
            // Bind to all network interfaces so phones/devices can access
            _server = CreateWebServer($"http://0.0.0.0:{port}/");
            await _server.RunAsync();
        }private WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithStaticFolder("/", _webRootPath, true)
                .WithAction("/payment", HttpVerbs.Get, HandlePaymentPage)
                .WithAction("/api/payment", HttpVerbs.Get, HandlePaymentDetailsApi)
                .WithAction("/confirm-payment", HttpVerbs.Post, HandlePaymentConfirmation)
                .WithAction("/api/register", HttpVerbs.Post, HandleRegistration)
                .WithAction("/api/payment", HttpVerbs.Options, HandleCorsPreflightRequest)
                .WithAction("/confirm-payment", HttpVerbs.Options, HandleCorsPreflightRequest);

            return server;
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
        }

        private async Task HandlePaymentPage(IHttpContext context)
        {
            var token = context.Request.QueryString["token"];
            var amount = context.Request.QueryString["amount"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(amount))
            {
                context.Response.StatusCode = 400;
                var errorBytes = Encoding.UTF8.GetBytes("Missing token or amount");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                return;
            }

            var payment = await _paymentService.GetPaymentByTokenAsync(token);
            if (payment == null)
            {
                context.Response.StatusCode = 404;
                var errorBytes = Encoding.UTF8.GetBytes("Payment not found");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                return;
            }

            // Serve the payment page with token and amount embedded
            var indexPath = Path.Combine(_webRootPath, "index.html");
            if (File.Exists(indexPath))
            {
                var content = await File.ReadAllTextAsync(indexPath);
                content = content.Replace("{{TOKEN}}", token).Replace("{{AMOUNT}}", amount);
                
                var responseBytes = Encoding.UTF8.GetBytes(content);
                context.Response.ContentType = "text/html";
                await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                context.Response.StatusCode = 500;
                var errorBytes = Encoding.UTF8.GetBytes("Payment page not found");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
            }        }        private async Task HandlePaymentDetailsApi(IHttpContext context)
        {
            AddCorsHeaders(context);
            
            var token = context.Request.QueryString["token"];
            var amount = context.Request.QueryString["amount"];
            var userId = context.Request.QueryString["userId"];

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 400;
                var errorBytes = Encoding.UTF8.GetBytes("{\"error\":\"Missing payment token\"}");
                context.Response.ContentType = "application/json";
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                return;
            }

            var payment = await _paymentService.GetPaymentByTokenAsync(token);
            if (payment == null)
            {
                context.Response.StatusCode = 404;
                var errorBytes = Encoding.UTF8.GetBytes("{\"error\":\"Payment not found or expired\"}");
                context.Response.ContentType = "application/json";
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                return;
            }

            // Return comprehensive payment details as JSON
            var paymentData = new
            {
                amount = payment.Amount.ToString("0.00"),
                description = payment.Description ?? "Library Account Recharge",
                user = $"User {payment.UserId}",
                token = token,
                status = payment.PaymentStatus.ToString(),
                createdDate = payment.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                paymentMethod = payment.PaymentMethod.ToString()
            };

            var jsonResponse = JsonSerializer.Serialize(paymentData);
            var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
            context.Response.ContentType = "application/json";
            await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
        }        private async Task HandlePaymentConfirmation(IHttpContext context)
        {
            AddCorsHeaders(context);
            
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                var body = await reader.ReadToEndAsync();
                var confirmationData = JsonSerializer.Deserialize<PaymentConfirmationData>(body);

                if (confirmationData?.Token == null)
                {
                    context.Response.StatusCode = 400;
                    var errorBytes = Encoding.UTF8.GetBytes("{\"success\":false,\"message\":\"Invalid request - missing token\"}");
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    return;
                }

                var success = await _paymentService.CompletePaymentAsync(confirmationData.Token);
                
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
            catch (Exception ex)
            {
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
        }

        private async Task HandleRegistration(IHttpContext context)
        {
            try
            {
                if (_authenticationService == null)
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

                var user = await _authenticationService.RegisterAsync(
                    registrationData.Username,
                    registrationData.Email,
                    registrationData.Password);

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
        }

        private class RegistrationData
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
        }
    }
}
