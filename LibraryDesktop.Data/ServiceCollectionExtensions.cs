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
                      .EnableSensitiveDataLogging()                      .EnableDetailedErrors());

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
    }
}
