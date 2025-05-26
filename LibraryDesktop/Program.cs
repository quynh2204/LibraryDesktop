using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using LibraryDesktop.View;
using LibraryDesktop.Data;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Data.Interfaces;
using System.Diagnostics;
using System.Windows.Forms;

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

            // Use a predictable location for the database file in the user's Documents folder
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var appDataPath = Path.Combine(documentsPath, "LibraryDesktop");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            var dbPath = Path.Combine(appDataPath, "Library.db");
            
            // Show the exact database path in the debug output
            Debug.WriteLine($"Database path: {dbPath}");
            MessageBox.Show($"Using database at: {dbPath}", "Database Location", MessageBoxButtons.OK, MessageBoxIcon.Information);

            try
            {
                // Configure services
                var host = CreateHostBuilder(dbPath).Build();
                
                // Initialize database
                bool dbInitialized = await InitializeDatabaseWithRetry(host.Services);
                
                if (!dbInitialized)
                {
                    MessageBox.Show("Could not initialize database. The application will now close.", 
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Run the main form with dependency injection
                using (var scope = host.Services.CreateScope())
                {
                    var mainForm = scope.ServiceProvider.GetRequiredService<Main>();
                    Application.Run(mainForm);
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
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Add Entity Framework and all library services with explicit database path
                    var connectionString = $"Data Source={dbPath}";
                    services.AddLibraryDataServices(connectionString);

                    // Add forms
                    services.AddTransient<Main>();
                    services.AddTransient<LoginForm>();
                    services.AddTransient<RegistrationForm>();
                    services.AddTransient<Exchange>();
                      
                    // Configure PaymentWebServer
                    services.AddSingleton<PaymentWebServer>(provider =>
                    {
                        var paymentService = provider.GetRequiredService<IPaymentService>();
                        var authenticationService = provider.GetRequiredService<IAuthenticationService>();
                        
                        // Look for WebRoot in several locations
                        string[] possiblePaths = {
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebRoot"),
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "WebRoot"),
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "LibraryDesktop.Data", "WebRoot")
                        };
                        
                        string webRootPath = possiblePaths.FirstOrDefault(Directory.Exists) ?? 
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
                        
                        Debug.WriteLine($"Using WebRoot path: {webRootPath}");
                        return new PaymentWebServer(paymentService, webRootPath, authenticationService);
                    });
                });
        }

        private static async Task<bool> InitializeDatabaseWithRetry(IServiceProvider serviceProvider)
        {
            const int maxRetries = 3;
            int attempt = 0;
            bool success = false;

            while (!success && attempt < maxRetries)
            {
                attempt++;
                try
                {
                    Debug.WriteLine($"Database initialization attempt {attempt}");
                    
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

                    // Force the database recreation on first run to ensure a clean state
                    if (attempt == 1)
                    {
                        Debug.WriteLine("Ensuring database is created...");
                        await context.Database.EnsureCreatedAsync();
                    }
                    
                    // Check database connection
                    var canConnect = await context.Database.CanConnectAsync();
                    Debug.WriteLine($"Can connect to database: {canConnect}");
                    
                    if (canConnect)
                    {
                        // Apply migrations
                        Debug.WriteLine("Applying migrations...");
                        await context.Database.MigrateAsync();
                        
                        // Verify database has tables
                        try
                        {
                            var userCount = await context.Users.CountAsync();
                            Debug.WriteLine($"Database contains {userCount} users");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error checking user count: {ex.Message}");
                            throw; // Re-throw to trigger retry
                        }
                        
                        Debug.WriteLine("Database initialization successful");
                        success = true;
                    }
                    else
                    {
                        Debug.WriteLine("Cannot connect to database, attempting to recreate it");
                        
                        // Try to ensure the database is deleted and recreated
                        try
                        {
                            await context.Database.EnsureDeletedAsync();
                            await context.Database.EnsureCreatedAsync();
                            await context.Database.MigrateAsync();
                            
                            Debug.WriteLine("Database recreated successfully");
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error recreating database: {ex.Message}");
                            throw; // Re-throw to trigger retry
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Database initialization error (attempt {attempt}): {ex.Message}");
                    Debug.WriteLine(ex.StackTrace);
                    
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        Debug.WriteLine(ex.InnerException.StackTrace);
                    }
                    
                    if (attempt >= maxRetries)
                    {
                        MessageBox.Show($"Failed to initialize database after {maxRetries} attempts. Error: {ex.Message}\n\nInner exception: {ex.InnerException?.Message}", 
                            "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        // Wait before retrying
                        await Task.Delay(1000 * attempt); // Increasing delay with each attempt
                    }
                }
            }
            
            return success;
        }
    }
}