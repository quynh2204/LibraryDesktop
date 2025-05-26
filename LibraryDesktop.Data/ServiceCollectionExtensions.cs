using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Data.Repositories;
using LibraryDesktop.Data.Services;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LibraryDesktop.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLibraryDataServices(this IServiceCollection services, string connectionString)
        {
            Debug.WriteLine($"Adding database services with connection string: {connectionString}");
            
            // Add DbContext with detailed logging
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlite(connectionString)
                      .LogTo(message => Debug.WriteLine($"EFCore: {message}"), 
                             LogLevel.Warning,
                             Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.LocalTime | 
                             Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.SingleLine)
                      .EnableSensitiveDataLogging()
                      .EnableDetailedErrors());

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add Repositories
            Debug.WriteLine("Adding repositories...");
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IUserSettingRepository, UserSettingRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // Add Services
            Debug.WriteLine("Adding services...");
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IGitHubContentService, GitHubContentService>();

            Debug.WriteLine("Library data services added successfully");
            return services;
        }

        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            Debug.WriteLine("Initializing database...");
            using var scope = serviceProvider.CreateScope();
            
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                
                // Verify the database file exists or can be created
                var connectionString = context.Database.GetConnectionString() ?? "";
                Debug.WriteLine($"Connection string: {connectionString}");
                
                var match = System.Text.RegularExpressions.Regex.Match(connectionString, @"Data Source=([^;]+)");
                if (match.Success)
                {
                    var dbPath = match.Groups[1].Value;
                    Debug.WriteLine($"Database file path: {dbPath}");
                    
                    var dbDirectory = Path.GetDirectoryName(dbPath);
                    if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
                    {
                        Debug.WriteLine($"Creating database directory: {dbDirectory}");
                        Directory.CreateDirectory(dbDirectory);
                    }
                }
                
                // Check database connection
                Debug.WriteLine("Checking database connection...");
                var canConnect = await context.Database.CanConnectAsync();
                Debug.WriteLine($"Can connect to database: {canConnect}");
                
                if (canConnect)
                {
                    // Run pending migrations if any
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    var migrationsList = string.Join(", ", pendingMigrations);
                    Debug.WriteLine($"Pending migrations: {(string.IsNullOrEmpty(migrationsList) ? "None" : migrationsList)}");
                    
                    if (pendingMigrations.Any())
                    {
                        Debug.WriteLine("Applying pending migrations...");
                        await context.Database.MigrateAsync();
                    }
                    
                    // Verify database contents
                    try
                    {
                        var userCount = await context.Users.CountAsync();
                        var bookCount = await context.Books.CountAsync();
                        Debug.WriteLine($"Database contains {userCount} users and {bookCount} books");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error verifying database content: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine("Creating new database...");
                    
                    // Ensure the database is created and migrations are applied
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                    await context.Database.MigrateAsync();
                    
                    Debug.WriteLine("Database created successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database initialization error: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Debug.WriteLine(ex.InnerException.StackTrace);
                }
                
                // Re-throw as we want to know about initialization failures
                throw new Exception($"Database initialization failed: {ex.Message}", ex);
            }
        }
    }
}
