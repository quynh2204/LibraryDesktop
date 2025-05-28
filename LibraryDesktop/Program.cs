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
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();        [STAThread]
        static async Task Main()
        {
            // Allocate console for debugging output
            AllocConsole();
            
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
                    var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();
                    
                    if (loginForm.ShowDialog() == DialogResult.OK && loginForm.AuthenticatedUser != null)
                    {
                        // User logged in successfully, show main form
                        var mainForm = scope.ServiceProvider.GetRequiredService<Main>();
                        
                        // Initialize main form with authenticated user
                        await mainForm.InitializeWithUserAsync(loginForm.AuthenticatedUser);
                        
                        Application.Run(mainForm);
                    }
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
                    services.AddTransient<Exchange>();
                    services.AddTransient<Dashboard>();
                    services.AddTransient<MyBooks>();
                    services.AddTransient<History>();
                    services.AddTransient<BookDetail>();                    // Configure PaymentWebServer - Register as Singleton but with Service Provider injection
                    services.AddSingleton<PaymentWebServer>(provider =>
                    {
                        // Use more reliable path resolution for WebRoot
                        var solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;
                        var webRootPath = Path.Combine(solutionDir ?? AppDomain.CurrentDomain.BaseDirectory, "LibraryDesktop.Data", "WebRoot");
                        
                        Console.WriteLine($"📁 WebRoot path: {webRootPath}");
                        Console.WriteLine($"📁 WebRoot exists: {Directory.Exists(webRootPath)}");
                        
                        // Pass the service provider instead of scoped services
                        return new PaymentWebServer(provider, webRootPath);
                    });
                });
        }        private static async Task InitializeDatabase(IServiceProvider serviceProvider, string dbPath)
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