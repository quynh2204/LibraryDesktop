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
            _server = CreateWebServer($"http://localhost:{port}/");
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
                .WithAction("/api/register", HttpVerbs.Post, HandleRegistration);

            return server;
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
            }        }

        private async Task HandlePaymentDetailsApi(IHttpContext context)
        {
            var token = context.Request.QueryString["token"];
            var amount = context.Request.QueryString["amount"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(amount))
            {
                context.Response.StatusCode = 400;
                var errorBytes = Encoding.UTF8.GetBytes("{\"error\":\"Missing token or amount\"}");
                context.Response.ContentType = "application/json";
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                return;
            }

            var payment = await _paymentService.GetPaymentByTokenAsync(token);
            if (payment == null)
            {
                context.Response.StatusCode = 404;
                var errorBytes = Encoding.UTF8.GetBytes("{\"error\":\"Payment not found\"}");
                context.Response.ContentType = "application/json";
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                return;
            }

            // Return payment details as JSON
            var paymentData = new
            {
                amount = amount,
                description = "Account recharge",
                user = $"User {payment.UserId}",
                token = token
            };

            var jsonResponse = JsonSerializer.Serialize(paymentData);
            var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
            context.Response.ContentType = "application/json";
            await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
        }

        private async Task HandlePaymentConfirmation(IHttpContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                var body = await reader.ReadToEndAsync();
                var confirmationData = JsonSerializer.Deserialize<PaymentConfirmationData>(body);

                if (confirmationData?.Token == null)
                {
                    context.Response.StatusCode = 400;
                    var errorBytes = Encoding.UTF8.GetBytes("Invalid request");
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    return;
                }

                var success = await _paymentService.CompletePaymentAsync(confirmationData.Token);
                var response = JsonSerializer.Serialize(new { success });
                var responseBytes = Encoding.UTF8.GetBytes(response);
                
                context.Response.ContentType = "application/json";
                await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                var errorBytes = Encoding.UTF8.GetBytes($"Error: {ex.Message}");
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
                var registrationData = JsonSerializer.Deserialize<RegistrationData>(body);

                if (registrationData?.Username == null || registrationData.Email == null || registrationData.Password == null)
                {
                    context.Response.StatusCode = 400;
                    var errorBytes = Encoding.UTF8.GetBytes("{\"error\":\"Missing required fields\"}");
                    context.Response.ContentType = "application/json";
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    return;
                }

                var user = await _authenticationService.RegisterAsync(registrationData.Username, registrationData.Email, registrationData.Password);
                
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
