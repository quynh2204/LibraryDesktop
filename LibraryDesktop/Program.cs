using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using LibraryDesktop.View;
using LibraryDesktop.Data;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Data.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibraryDesktop
{
    internal static class Program
    {
        [STAThread]
        static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // Use LocalApplicationData for better performance and consistency with DbContext
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LibraryDesktop");

            // Create directory if it doesn't exist
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            var dbPath = Path.Combine(appDataPath, "Library.db");

            // Show the exact database path in the debug output
            Debug.WriteLine($"Database path: {dbPath}");            try
            {
                // Configure services
                var host = CreateHostBuilder(dbPath).Build();

                // Initialize database
                await InitializeDatabase(host.Services, dbPath);                // Start the application with LoginForm
                using (var scope = host.Services.CreateScope())
                {
                    DialogResult loginResult;
                    do
                    {
                        var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();
                        loginResult = loginForm.ShowDialog();

                        if (loginResult == DialogResult.OK && loginForm.AuthenticatedUser != null)
                        {
                            // User logged in successfully, show main form
                            var mainForm = scope.ServiceProvider.GetRequiredService<Main>();

                            // Initialize main form with authenticated user
                            await mainForm.InitializeWithUserAsync(loginForm.AuthenticatedUser);

                            Application.Run(mainForm);
                            break; // Exit the loop and application
                        }
                        // If loginResult is Retry, continue the loop to show login form again
                        // If loginResult is Cancel or anything else, exit the loop
                    } while (loginResult == DialogResult.Retry);
                    
                    // If login was cancelled or failed, application exits
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal error: {ex.Message}\n\n{ex.StackTrace}",
                    "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static IHostBuilder CreateHostBuilder(string dbPath)
        {
            return Host.CreateDefaultBuilder()                .ConfigureServices((context, services) =>
                {
                    // Add Entity Framework and all library services
                    services.AddLibraryDataServices($"Data Source={dbPath}");                    // Add forms
                    services.AddTransient<Main>();
                    services.AddTransient<LoginForm>();
                    services.AddTransient<RegistrationForm>();
                    services.AddTransient<ResetPasswordForm>();
                    services.AddTransient<Home>();
                    services.AddTransient<Exchange>();
                    services.AddTransient<Dashboard>();
                    services.AddTransient<MyBooks>();
                    services.AddTransient<History>();
                    services.AddTransient<BookDetail>();
                      // Configure PaymentWebServer
                    services.AddSingleton<PaymentWebServer>(provider =>
                    {
                        var paymentService = provider.GetRequiredService<IPaymentService>();
                        var authenticationService = provider.GetRequiredService<IAuthenticationService>();
                        var solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;
                        var webRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "LibraryDesktop.Data", "WebRoot");
                        Console.WriteLine($"📁 WebRoot path: {webRootPath}");
                        Console.WriteLine($"📁 WebRoot exists: {Directory.Exists(webRootPath)}");
                        return new PaymentWebServer(provider, webRootPath);
                    });
                });
        }
        private static async Task InitializeDatabase(IServiceProvider serviceProvider, string dbPath)
        {
            try
            {
                Debug.WriteLine("Initializing database...");

                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

                // Ensure database exists and migrations are applied
                await context.Database.MigrateAsync();
                Debug.WriteLine("Database migration completed successfully");

                // Verify database has tables
                var userCount = await context.Users.CountAsync();
                Debug.WriteLine($"Database contains {userCount} users");

                Debug.WriteLine("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database initialization error: {ex.Message}");
                MessageBox.Show($"Database initialization failed: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}