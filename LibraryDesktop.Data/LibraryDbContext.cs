using Microsoft.EntityFrameworkCore;
using LibraryDesktop.Models;
using System.Diagnostics;

namespace LibraryDesktop.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext()
        {
            // Add debug output for constructor
            Debug.WriteLine("LibraryDbContext parameterless constructor called");
        }
        
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
            // Add debug output for options constructor
            Debug.WriteLine("LibraryDbContext options constructor called");
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<Payment> Payments { get; set; }        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use a more explicit default connection string with a full path if needed
                var dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "LibraryDesktop", 
                    "Library.db");
                
                // Ensure directory exists
                var dbDir = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir!);
                }
                
                Debug.WriteLine($"DbContext OnConfiguring using path: {dbPath}");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.EnableDetailedErrors();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Debug.WriteLine("Creating database model...");

            // Configure relationships and constraints
            try
            {
                ConfigureUserEntity(modelBuilder);
                ConfigureCategoryEntity(modelBuilder);
                ConfigureBookEntity(modelBuilder);
                ConfigureChapterEntity(modelBuilder);
                ConfigureUserFavoriteEntity(modelBuilder);
                ConfigureRatingEntity(modelBuilder);
                ConfigureUserSettingEntity(modelBuilder);
                ConfigurePaymentEntity(modelBuilder);

                // Seed initial data
                SeedData(modelBuilder);
                
                Debug.WriteLine("Database model created successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnModelCreating: {ex.Message}");
                throw; // Re-throw to make the error visible
            }
        }

        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.AvatarUrl).HasMaxLength(255);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }

        private void ConfigureCategoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.CategoryName).IsUnique();
            });
        }

        private void ConfigureBookEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(e => e.BookId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Books)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureChapterEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.ToTable("Chapters");
                entity.HasKey(e => e.ChapterId);
                entity.Property(e => e.ChapterTitle).IsRequired().HasMaxLength(200);
                entity.Property(e => e.GitHubContentUrl).IsRequired().HasMaxLength(500);
                
                entity.HasOne(e => e.Book)
                    .WithMany(b => b.Chapters)
                    .HasForeignKey(e => e.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasIndex(e => new { e.BookId, e.ChapterNumber }).IsUnique();
            });
        }

        private void ConfigureUserFavoriteEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFavorite>(entity =>
            {
                entity.ToTable("UserFavorites");
                entity.HasKey(e => e.FavoriteId);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Book)
                    .WithMany(b => b.Favorites)
                    .HasForeignKey(e => e.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasIndex(e => new { e.UserId, e.BookId }).IsUnique();
            });
        }

        private void ConfigureRatingEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Ratings");
                entity.HasKey(e => e.RatingId);
                entity.Property(e => e.Review).HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Ratings)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Book)
                    .WithMany(b => b.Ratings)
                    .HasForeignKey(e => e.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasIndex(e => new { e.UserId, e.BookId }).IsUnique();
            });
        }

        private void ConfigureUserSettingEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSetting>(entity =>
            {
                entity.ToTable("UserSettings");
                entity.HasKey(e => e.SettingId);
                entity.Property(e => e.Balance).HasColumnType("decimal(10,2)");
                
                entity.HasOne(e => e.User)
                    .WithOne(u => u.UserSetting)
                    .HasForeignKey<UserSetting>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigurePaymentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.PaymentToken).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasIndex(e => e.PaymentToken).IsUnique();
            });
        }        private void SeedData(ModelBuilder modelBuilder)
        {
            // Use a static date to avoid migration issues
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Debug.WriteLine("Seeding initial data...");
            
            try
            {
                // Seed Categories
                modelBuilder.Entity<Category>().HasData(
                    new Category { CategoryId = 1, CategoryName = "Fantasy", Description = "Fantasy stories and novels", CreatedDate = seedDate, IsActive = true },
                    new Category { CategoryId = 2, CategoryName = "Romance", Description = "Romance stories and novels", CreatedDate = seedDate, IsActive = true },
                    new Category { CategoryId = 3, CategoryName = "Sci-Fi", Description = "Science fiction stories", CreatedDate = seedDate, IsActive = true },
                    new Category { CategoryId = 4, CategoryName = "Mystery", Description = "Mystery and thriller stories", CreatedDate = seedDate, IsActive = true },
                    new Category { CategoryId = 5, CategoryName = "Adventure", Description = "Adventure stories", CreatedDate = seedDate, IsActive = true }
                );

                // Seed Demo User (password: "demo")
                modelBuilder.Entity<User>().HasData(
                    new User 
                    { 
                        UserId = 1, 
                        Username = "demo", 
                        Email = "demo@library.com", 
                        PasswordHash = "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", // SHA256 hash of "demo" + salt
                        RegistrationDate = seedDate
                    }
                );
                
                // Seed Demo User Settings
                modelBuilder.Entity<UserSetting>().HasData(
                    new UserSetting
                    {
                        SettingId = 1,
                        UserId = 1,
                        ThemeMode = ThemeMode.Light,
                        FontSize = 12,
                        Balance = 100.00m
                    }
                );
                
                Debug.WriteLine("Initial data seeded successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error seeding data: {ex.Message}");
                // Continue without throwing to allow the database to be created even if seeding fails
            }
        }
    }
}
