using Microsoft.EntityFrameworkCore;
using LibraryDesktop.Models;
using System.Diagnostics;

namespace LibraryDesktop.Data
{    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext()
        {
        }
        
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
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
                // Use LocalApplicationData to match Program.cs configuration
                var dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "LibraryDesktop",
                    "Library.db");

                Debug.WriteLine($"DbContext OnConfiguring using path: {dbPath}");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
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
        }        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.AvatarUrl).HasMaxLength(255);
                entity.Property(e => e.Coins).HasDefaultValue(0);
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
        }        private void ConfigureUserSettingEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSetting>(entity =>
            {
                entity.ToTable("UserSettings");
                entity.HasKey(e => e.SettingId);
                
                entity.HasOne(e => e.User)
                    .WithMany() // Remove one-to-one relationship since User no longer has UserSetting navigation
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigurePaymentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasColumnType("INTEGER");
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
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Fantasy", Description = "Fantasy stories and novels", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 2, CategoryName = "Romance", Description = "Romance stories and novels", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 3, CategoryName = "Sci-Fi", Description = "Science fiction stories", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 4, CategoryName = "Mystery", Description = "Mystery and thriller stories", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 5, CategoryName = "Adventure", Description = "Adventure stories", CreatedDate = seedDate, IsActive = true }
            );            // Seed Demo User
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    UserId = 1, 
                    Username = "demo", 
                    Email = "demo@library.com", 
                    PasswordHash = "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", // SHA256 hash of "demo" + salt
                    RegistrationDate = seedDate,
                    Coins = 100
                }
            );// Seed Demo User Settings
            modelBuilder.Entity<UserSetting>().HasData(
                new UserSetting
                {
                    SettingId = 1,
                    UserId = 1,
                    ThemeMode = ThemeMode.Light,
                    FontSize = 12
                }
            );// Seed Books with Asset Images
            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "The Enchanted Forest", Author = "Elena Moonstone", CategoryId = 1, Description = "A magical journey through an ancient forest filled with mystical creatures and forgotten secrets.", Price = 15.99m, CoverImageUrl = "Assets/0d080b47aaa3ab11160e60091f5ecbb7.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 2, Title = "Love in the City", Author = "Sarah Martinez", CategoryId = 2, Description = "A heartwarming romance set in the bustling streets of New York City.", Price = 12.99m, CoverImageUrl = "Assets/2ef1ef06a27bf5cd68fea90a24cc96dd.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 3, Title = "Quantum Horizons", Author = "Dr. Michael Chen", CategoryId = 3, Description = "An epic science fiction adventure exploring the boundaries of space and time.", Price = 18.99m, CoverImageUrl = "Assets/3398eb12b32fa930e105e701b708bc9a.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 4, Title = "The Shadow Detective", Author = "James Blackwood", CategoryId = 4, Description = "A gripping mystery thriller following Detective Morgan through the darkest corners of the city.", Price = 14.99m, CoverImageUrl = "Assets/5cb878e981ec841cf8963c2dbfc837c3.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 5, Title = "Mountain Quest", Author = "Adventure Kelly", CategoryId = 5, Description = "An thrilling adventure story of climbing the world's most dangerous peaks.", Price = 16.99m, CoverImageUrl = "Assets/65b07f0ccb5631d4025d509c0c14e62d.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 6, Title = "Dragon's Legacy", Author = "Aria Dragonheart", CategoryId = 1, Description = "The epic tale of the last dragon rider and their quest to save the realm.", Price = 19.99m, CoverImageUrl = "Assets/6a81c3d24a73711e02ba8593c067bccf.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 7, Title = "Starbound Lovers", Author = "Luna Starfield", CategoryId = 2, Description = "A cosmic romance spanning galaxies and defying the laws of physics.", Price = 13.99m, CoverImageUrl = "Assets/9a321b2c38deed11aa8fb0e879cc6610.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 8, Title = "Time Paradox", Author = "Prof. Alexandra Time", CategoryId = 3, Description = "A mind-bending sci-fi thriller about time travel and its consequences.", Price = 17.99m, CoverImageUrl = "Assets/c879fb508a3217ace62142bf6f7b72c1.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 9, Title = "The Lost Cipher", Author = "Rebecca Stone", CategoryId = 4, Description = "An ancient code holds the key to preventing a global catastrophe.", Price = 15.99m, CoverImageUrl = "Assets/e19d44be068aeef341ef687ce43ce5a3.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 10, Title = "Ocean Explorer", Author = "Captain Marina Blue", CategoryId = 5, Description = "Dive into the deepest mysteries of the ocean in this underwater adventure.", Price = 14.99m, CoverImageUrl = "Assets/e3c33ed5f3d5d99567ae20bd138aa913.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 11, Title = "The Wizard's Apprentice", Author = "Merlin Wiseheart", CategoryId = 1, Description = "A young apprentice discovers hidden powers and ancient magical secrets.", Price = 16.99m, CoverImageUrl = "Assets/f0e51f4a153d25e0438d429892ac8fa6.jpg", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 12, Title = "Midnight Romance", Author = "Scarlett Dreams", CategoryId = 2, Description = "A passionate love story that blooms under the moonlit sky.", Price = 11.99m, CoverImageUrl = "Assets/f4c232745d79b53ac510d102a347fb4b.jpg", Status = BookStatus.Published, CreatedDate = seedDate }
            );            // Seed Chapters for some books
            modelBuilder.Entity<Chapter>().HasData(
                new Chapter { ChapterId = 1, BookId = 1, ChapterNumber = 1, ChapterTitle = "The Mysterious Path", GitHubContentUrl = "https://github.com/example/enchanted-forest/chapter1.md", PublishedDate = seedDate },
                new Chapter { ChapterId = 2, BookId = 1, ChapterNumber = 2, ChapterTitle = "Meeting the Guardian", GitHubContentUrl = "https://github.com/example/enchanted-forest/chapter2.md", PublishedDate = seedDate },
                new Chapter { ChapterId = 3, BookId = 2, ChapterNumber = 1, ChapterTitle = "First Encounter", GitHubContentUrl = "https://github.com/example/love-city/chapter1.md", PublishedDate = seedDate },
                new Chapter { ChapterId = 4, BookId = 3, ChapterNumber = 1, ChapterTitle = "The Quantum Discovery", GitHubContentUrl = "https://github.com/example/quantum-horizons/chapter1.md", PublishedDate = seedDate }
            );
        }
    }
}
