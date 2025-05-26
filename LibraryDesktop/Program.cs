using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using LibraryDesktop.View;
using LibraryDesktop.Data;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Data.Interfaces;

namespace LibraryDesktop
{
    internal static class Program
    {        [STAThread]
        static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configure services
            var host = CreateHostBuilder().Build();
            
            // Initialize database
            await host.Services.InitializeDatabaseAsync();
            
            // Run the main form with dependency injection
            using (var scope = host.Services.CreateScope())
            {
                var mainForm = scope.ServiceProvider.GetRequiredService<Main>();
                Application.Run(mainForm);
            }
        }static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()                .ConfigureServices((context, services) =>
                {
                    // Add Entity Framework and all library services
                    services.AddLibraryDataServices("Data Source=Library.db");                    // Add forms
                    services.AddTransient<Main>();
                    services.AddTransient<LoginForm>();
                    services.AddTransient<RechargeForm>();
                    services.AddTransient<RegistrationForm>();
                      // Configure PaymentWebServer
                    services.AddSingleton<PaymentWebServer>(provider =>
                    {
                        var paymentService = provider.GetRequiredService<IPaymentService>();
                        var authenticationService = provider.GetRequiredService<IAuthenticationService>();
                        var webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "LibraryDesktop.Data", "WebRoot");
                        return new PaymentWebServer(paymentService, webRootPath, authenticationService);
                    });
                });
        }
    }
}