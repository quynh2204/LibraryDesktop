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
            // Add Entity Framework
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlite(connectionString));

            // Add HttpClient for GitHub content downloading
            services.AddHttpClient<IGitHubContentService, GitHubContentService>();

            // Add DbContext
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlite(connectionString)
                      .LogTo(message => Debug.WriteLine($"EFCore: {message}"), 
                             LogLevel.Warning,
                             Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.LocalTime | 
                             Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.SingleLine)
                      .EnableSensitiveDataLogging()                      .EnableDetailedErrors());

            // Add Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IUserSettingRepository, UserSettingRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();


            // Add Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGitHubContentService, GitHubContentService>();

            return services;
        }        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            
            try
            {
                // Check if database exists and has tables
                var canConnect = await context.Database.CanConnectAsync();
                if (canConnect)
                {
                    // Only run pending migrations if database exists
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        await context.Database.MigrateAsync();
                    }
                }
                else
                {
                    // Create database and run all migrations
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                System.Diagnostics.Debug.WriteLine($"Database initialization warning: {ex.Message}");
            }
        }
    }
}
