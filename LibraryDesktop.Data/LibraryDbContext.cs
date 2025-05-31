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
        }        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<History> Histories { get; set; }protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
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
            base.OnModelCreating(modelBuilder);            // Configure relationships and constraints
            ConfigureUserEntity(modelBuilder);
            ConfigureCategoryEntity(modelBuilder);
            ConfigureBookEntity(modelBuilder);
            ConfigureChapterEntity(modelBuilder);
            ConfigureUserFavoriteEntity(modelBuilder);
            ConfigureRatingEntity(modelBuilder);
            ConfigureUserSettingEntity(modelBuilder);
            ConfigurePaymentEntity(modelBuilder);
            ConfigureHistoryEntity(modelBuilder);

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
            });        }

        private void ConfigureHistoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("Histories");
                entity.HasKey(e => e.HistoryId);
                entity.Property(e => e.AccessType).IsRequired().HasMaxLength(50);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Book)
                    .WithMany()
                    .HasForeignKey(e => e.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Chapter)
                    .WithMany()
                    .HasForeignKey(e => e.ChapterId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }        
        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Giả Tưởng", Description = "Truyện và tiểu thuyết giả tưởng", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 2, CategoryName = "Văn học hiện đại", Description = "Truyện và tác phẩm văn học hiện đại", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 3, CategoryName = "Kinh tế", Description = "Sách và truyện về lĩnh vực kinh tế", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 4, CategoryName = "Trinh thám", Description = "Truyện và tiểu thuyết trinh thám", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 5, CategoryName = "Lịch sử", Description = "Sách và truyện về lịch sử", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 6, CategoryName = "Triết học", Description = "Tiểu thuyết và sách về triết học", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 7, CategoryName = "Kỹ năng sống", Description = "Sách về kỹ năng sống", CreatedDate = seedDate, IsActive = true },
                new Category { CategoryId = 8, CategoryName = "Khoa học viễn tưởng", Description = "Truyện và tiểu thuyết khoa học viễn tưởng", CreatedDate = seedDate, IsActive = true }
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
            );            // Seed Books with Asset Images
            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "Harry Potter và Hòn đá Phù thủy", Author="J.K. Rowling", CategoryId = 1, Description = "Câu chuyện về cậu bé phù thủy Harry Potter và cuộc phiêu lưu đầu tiên tại trường Hogwarts. Khám phá thế giới phép thuật đầy kỳ diệu và những người bạn đồng hành.", ViewCount = 70, Price = 0, CoverImageUrl = "LibraryDesktop/Assets/1.png", Status = BookStatus.Completed, CreatedDate = seedDate},
                new Book { BookId = 2, Title = "Tôi thấy hoa vàng trên cỏ xanh", Author = "Nguyễn Nhật Ánh", CategoryId = 2, Description = "Tác phẩm kể về tuổi thơ của những đứa trẻ miền quê, với những kỷ niệm đẹp về tình anh em, tình làng nghĩa xóm và những bài học cuộc sống quý giá.", ViewCount = 50, Price = 0, CoverImageUrl = "LibraryDesktop/Assets/2.png", Status = BookStatus.Completed, CreatedDate = seedDate },
                new Book { BookId = 3, Title = "Sherlock Holmes: Cuộc phiêu lưu của Sherlock Holmes", Author = "Arthur Conan Doyle", CategoryId = 4, Description = "Tuyển tập những vụ án kinh điển của thám tử vĩ đại Sherlock Holmes và người bạn đồng hành Watson. Những câu chuyện trinh thám hấp dẫn và đầy bí ẩn.", ViewCount = 25, Price = 0, CoverImageUrl = "LibraryDesktop/Assets/3.png", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 4, Title = "Dạy con làm giàu", Author = "Robert Kiyosaki", CategoryId = 3, Description = "Cuốn sách dạy về tư duy tài chính và cách quản lý tiền bạc hiệu quả. Những bài học quý giá về đầu tư và xây dựng tài sản.", ViewCount = 30, Price = 50, CoverImageUrl = "LibraryDesktop/Assets/4.png", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 5, Title = "Lịch sử Việt Nam: Đại Việt sử ký toàn thư", Author = "Ngô Sĩ Liên", CategoryId = 5, Description = "Tác phẩm sử học quan trọng ghi chép lịch sử Việt Nam từ thời cổ đại đến thế kỷ XV. Nguồn tài liệu quý giá về văn hóa và lịch sử dân tộc.", ViewCount = 35, Price = 0, CoverImageUrl = "LibraryDesktop/Assets/5.png", Status = BookStatus.Completed, CreatedDate = seedDate },
                new Book { BookId = 6, Title = "Nhà giả kim", Author = "Paulo Coelho", CategoryId = 6, Description = "Câu chuyện về chàng chăn cừu Santiago và cuộc hành trình tìm kiếm kho báu. Một tác phẩm triết lý sâu sắc về ước mơ và ý nghĩa cuộc sống.", ViewCount = 40, Price = 50, CoverImageUrl = "LibraryDesktop/Assets/6.png", Status = BookStatus.Published, CreatedDate = seedDate },
                new Book { BookId = 7, Title = "Đắc nhân tâm", Author = "Dale Carnegie", CategoryId = 7, Description = "Cuốn sách kinh điển về nghệ thuật giao tiếp và ứng xử. Hướng dẫn cách xây dựng mối quan hệ tốt và thành công trong cuộc sống.", ViewCount = 45, Price = 50, CoverImageUrl = "LibraryDesktop/Assets/7.png", Status = BookStatus.Published, CreatedDate = seedDate }
            );// Seed Chapters for some books
            modelBuilder.Entity<Chapter>().HasData(
                new Chapter { ChapterId = 1, BookId = 1, ChapterNumber = 1, ChapterTitle = "Đứa bé vẫn sống", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2001.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 2, BookId = 1, ChapterNumber = 2, ChapterTitle = "Tấm kính biến mất", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2002.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 3, BookId = 1, ChapterNumber = 3, ChapterTitle = "Những lá thư không xuất xứ", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2003.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 4, BookId = 1, ChapterNumber = 4, ChapterTitle = "Người giữ khóa", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2004.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 5, BookId = 1, ChapterNumber = 5, ChapterTitle = "Hẻm Xéo", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2005.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 6, BookId = 2, ChapterNumber = 1, ChapterTitle = "Hoa tay", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/T%C3%B4i%20Th%E1%BA%A5y%20Hoa%20V%C3%A0ng%20Tr%C3%AAn%20C%E1%BB%8F%20Xanh/Ch%C6%B0%C6%A1ng%2001.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 7, BookId = 2, ChapterNumber = 2, ChapterTitle = "Những ngón tay", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/T%C3%B4i%20Th%E1%BA%A5y%20Hoa%20V%C3%A0ng%20Tr%C3%AAn%20C%E1%BB%8F%20Xanh/Ch%C6%B0%C6%A1ng%2002.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 8, BookId = 2, ChapterNumber = 3, ChapterTitle = "Chú Đàn", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/T%C3%B4i%20Th%E1%BA%A5y%20Hoa%20V%C3%A0ng%20Tr%C3%AAn%20C%E1%BB%8F%20Xanh/Ch%C6%B0%C6%A1ng%2003.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 9, BookId = 2, ChapterNumber = 4, ChapterTitle = "Chuyện ma của chú Đàn", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/T%C3%B4i%20Th%E1%BA%A5y%20Hoa%20V%C3%A0ng%20Tr%C3%AAn%20C%E1%BB%8F%20Xanh/Ch%C6%B0%C6%A1ng%2004.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 10, BookId = 3, ChapterNumber = 1, ChapterTitle = "Dải băng lốm đốm", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2001.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 11, BookId = 3, ChapterNumber = 2, ChapterTitle = "Hội tóc hung", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2002.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 12, BookId = 3, ChapterNumber = 3, ChapterTitle = "Bí ẩn ở thung lũng", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2003.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 13, BookId = 3, ChapterNumber = 4, ChapterTitle = "Năm hột cam", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2004.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 14, BookId = 3, ChapterNumber = 5, ChapterTitle = "Chiếc vương miện", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2005.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 15, BookId = 4, ChapterNumber = 1, ChapterTitle = "Cha giàu, cha nghèo", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/D%E1%BA%A1y%20Con%20L%C3%A0m%20Gi%C3%A0u%20-%20T%E1%BA%ADp%201/Ch%C6%B0%C6%A1ng%2001.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 16, BookId = 4, ChapterNumber = 2, ChapterTitle = "Người giàu không làm việc vì tiền", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/D%E1%BA%A1y%20Con%20L%C3%A0m%20Gi%C3%A0u%20-%20T%E1%BA%ADp%201/Ch%C6%B0%C6%A1ng%2002.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 17, BookId = 4, ChapterNumber = 3, ChapterTitle = "Tại sao phải dạy con về tài chính?", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/D%E1%BA%A1y%20Con%20L%C3%A0m%20Gi%C3%A0u%20-%20T%E1%BA%ADp%201/Ch%C6%B0%C6%A1ng%2003.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 18, BookId = 5, ChapterNumber = 1, ChapterTitle = "Đại Việt Sử Ký Ngoại Kỷ Toàn Thư: Quyển I", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/L%E1%BB%8Bch%20S%E1%BB%AD%20Vi%E1%BB%87t%20Nam%20-%20%C4%90%E1%BA%A1i%20Vi%E1%BB%87t%20S%E1%BB%AD%20K%C3%BD%20To%C3%A0n%20Th%C6%B0/Quy%E1%BB%83n%20I.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 19, BookId = 5, ChapterNumber = 2, ChapterTitle = "Đại Việt Sử Ký Ngoại Kỷ Toàn Thư: Quyển II", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/L%E1%BB%8Bch%20S%E1%BB%AD%20Vi%E1%BB%87t%20Nam%20-%20%C4%90%E1%BA%A1i%20Vi%E1%BB%87t%20S%E1%BB%AD%20K%C3%BD%20To%C3%A0n%20Th%C6%B0/Quy%E1%BB%83n%20II.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 20, BookId = 5, ChapterNumber = 3, ChapterTitle = "Đại Việt Sử Ký Ngoại Kỷ Toàn Thư: Quyển III", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/L%E1%BB%8Bch%20S%E1%BB%AD%20Vi%E1%BB%87t%20Nam%20-%20%C4%90%E1%BA%A1i%20Vi%E1%BB%87t%20S%E1%BB%AD%20K%C3%BD%20To%C3%A0n%20Th%C6%B0/Quy%E1%BB%83n%20III.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 21, BookId = 6, ChapterNumber = 1, ChapterTitle = "Chương 1", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Nh%C3%A0%20Gi%E1%BA%A3%20Kim/Ch%C6%B0%C6%A1ng%2001.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 22, BookId = 6, ChapterNumber = 2, ChapterTitle = "Chương 2", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Nh%C3%A0%20Gi%E1%BA%A3%20Kim/Ch%C6%B0%C6%A1ng%2002.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 23, BookId = 6, ChapterNumber = 3, ChapterTitle = "Chương 3", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Nh%C3%A0%20Gi%E1%BA%A3%20Kim/Ch%C6%B0%C6%A1ng%2003.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 24, BookId = 6, ChapterNumber = 4, ChapterTitle = "Chương 4", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Nh%C3%A0%20Gi%E1%BA%A3%20Kim/Ch%C6%B0%C6%A1ng%2004.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 25, BookId = 7, ChapterNumber = 1, ChapterTitle = "Muốn lấy mật thì đừng phá tổ ong", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/%C4%90%E1%BA%AFc%20Nh%C3%A2n%20T%C3%A2m/Ch%C6%B0%C6%A1ng%2001.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 26, BookId = 7, ChapterNumber = 2, ChapterTitle = "Bí mật lớn nhất trong phép ứng xử", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/%C4%90%E1%BA%AFc%20Nh%C3%A2n%20T%C3%A2m/Ch%C6%B0%C6%A1ng%2002.txt", PublishedDate = seedDate },
                new Chapter { ChapterId = 27, BookId = 7, ChapterNumber = 3, ChapterTitle = "Ai làm được điều dưới đây", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/%C4%90%E1%BA%AFc%20Nh%C3%A2n%20T%C3%A2m/Ch%C6%B0%C6%A1ng%2003.txt", PublishedDate = seedDate },                new Chapter { ChapterId = 28, BookId = 7, ChapterNumber = 4, ChapterTitle = "Thành thật quan tâm đến người khác", GitHubContentUrl = "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/%C4%90%E1%BA%AFc%20Nh%C3%A2n%20T%C3%A2m/Ch%C6%B0%C6%A1ng%2004.txt", PublishedDate = seedDate }
            );

            // Seed additional demo users for ratings
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    UserId = 2, 
                    Username = "alice", 
                    Email = "alice@library.com", 
                    PasswordHash = "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", 
                    RegistrationDate = seedDate.AddDays(-30),
                    Coins = 150
                },
                new User 
                { 
                    UserId = 3, 
                    Username = "bob", 
                    Email = "bob@library.com", 
                    PasswordHash = "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", 
                    RegistrationDate = seedDate.AddDays(-20),
                    Coins = 80
                },
                new User 
                { 
                    UserId = 4, 
                    Username = "carol", 
                    Email = "carol@library.com", 
                    PasswordHash = "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", 
                    RegistrationDate = seedDate.AddDays(-15),
                    Coins = 200
                },
                new User 
                { 
                    UserId = 5, 
                    Username = "david", 
                    Email = "david@library.com", 
                    PasswordHash = "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", 
                    RegistrationDate = seedDate.AddDays(-10),
                    Coins = 120
                }
            );            // Seed sample ratings with comments
            modelBuilder.Entity<Rating>().HasData(
                // Harry Potter ratings
                new Rating { RatingId = 1, UserId = 2, BookId = 1, RatingValue = 5, Review = "Cuốn sách tuyệt vời! Câu chuyện phù thủy rất hấp dẫn và đầy màu sắc. Tôi đã đọc nhiều lần và vẫn thích.", CreatedDate = seedDate.AddDays(-25), UpdatedDate = seedDate.AddDays(-25) },
                new Rating { RatingId = 2, UserId = 3, BookId = 1, RatingValue = 4, Review = "Hay nhưng hơi dài dòng ở một số đoạn. Nhìn chung vẫn là một tác phẩm đáng đọc.", CreatedDate = seedDate.AddDays(-22), UpdatedDate = seedDate.AddDays(-22) },
                new Rating { RatingId = 3, UserId = 4, BookId = 1, RatingValue = 5, Review = "Thế giới phép thuật được xây dựng rất chi tiết và logic. J.K. Rowling thực sự là một thiên tài!", CreatedDate = seedDate.AddDays(-18), UpdatedDate = seedDate.AddDays(-18) },
                
                // Tôi thấy hoa vàng trên cỏ xanh ratings
                new Rating { RatingId = 4, UserId = 2, BookId = 2, RatingValue = 5, Review = "Nguyễn Nhật Ánh viết về tuổi thơ rất chân thực và cảm động. Đọc xong như quay về thời thơ ấu.", CreatedDate = seedDate.AddDays(-20), UpdatedDate = seedDate.AddDays(-20) },
                new Rating { RatingId = 5, UserId = 5, BookId = 2, RatingValue = 4, Review = "Câu chuyện đẹp về tình anh em và làng quê Việt Nam. Có những đoạn khiến tôi rơi nước mắt.", CreatedDate = seedDate.AddDays(-16), UpdatedDate = seedDate.AddDays(-16) },
                
                // Sherlock Holmes ratings
                new Rating { RatingId = 6, UserId = 3, BookId = 3, RatingValue = 5, Review = "Trinh thám kinh điển! Sherlock Holmes thông minh và các vụ án rất logic. Không thể bỏ xuống được.", CreatedDate = seedDate.AddDays(-19), UpdatedDate = seedDate.AddDays(-19) },
                new Rating { RatingId = 7, UserId = 4, BookId = 3, RatingValue = 4, Review = "Phong cách viết của Conan Doyle rất hấp dẫn. Mỗi câu chuyện đều có twist bất ngờ.", CreatedDate = seedDate.AddDays(-14), UpdatedDate = seedDate.AddDays(-14) },
                new Rating { RatingId = 8, UserId = 5, BookId = 3, RatingValue = 5, Review = "Đây là lý do tại sao Sherlock Holmes trở thành biểu tượng thám tử. Xuất sắc!", CreatedDate = seedDate.AddDays(-12), UpdatedDate = seedDate.AddDays(-12) },
                
                // Dạy con làm giàu ratings
                new Rating { RatingId = 9, UserId = 2, BookId = 4, RatingValue = 5, Review = "Cuốn sách thay đổi tư duy của tôi về tiền bạc và đầu tư. Rất thực tế và dễ hiểu.", CreatedDate = seedDate.AddDays(-17), UpdatedDate = seedDate.AddDays(-17) },
                new Rating { RatingId = 10, UserId = 3, BookId = 4, RatingValue = 3, Review = "Có nhiều ý hay nhưng một số quan điểm hơi Mỹ hóa. Cần điều chỉnh cho phù hợp Việt Nam.", CreatedDate = seedDate.AddDays(-13), UpdatedDate = seedDate.AddDays(-13) },
                new Rating { RatingId = 11, UserId = 5, BookId = 4, RatingValue = 4, Review = "Bài học về tài chính cá nhân rất bổ ích. Đáng đọc cho mọi lứa tuổi.", CreatedDate = seedDate.AddDays(-10), UpdatedDate = seedDate.AddDays(-10) },
                
                // Lịch sử Việt Nam ratings
                new Rating { RatingId = 12, UserId = 4, BookId = 5, RatingValue = 4, Review = "Tài liệu lịch sử quý giá, giúp hiểu rõ hơn về nguồn gốc dân tộc. Hơi khó đọc với người hiện đại.", CreatedDate = seedDate.AddDays(-11), UpdatedDate = seedDate.AddDays(-11) },
                new Rating { RatingId = 13, UserId = 5, BookId = 5, RatingValue = 5, Review = "Đại Việt sử ký là kho tàng văn hóa dân tộc. Mọi người Việt Nam nên đọc ít nhất một lần.", CreatedDate = seedDate.AddDays(-8), UpdatedDate = seedDate.AddDays(-8) },
                
                // Nhà giả kim ratings  
                new Rating { RatingId = 14, UserId = 2, BookId = 6, RatingValue = 5, Review = "Triết lý sâu sắc về cuộc sống và ước mơ. Paulo Coelho viết rất hay và ý nghĩa.", CreatedDate = seedDate.AddDays(-15), UpdatedDate = seedDate.AddDays(-15) },
                new Rating { RatingId = 15, UserId = 3, BookId = 6, RatingValue = 4, Review = "Câu chuyện đơn giản nhưng chứa đựng nhiều bài học nhân sinh. Đáng suy ngẫm.", CreatedDate = seedDate.AddDays(-9), UpdatedDate = seedDate.AddDays(-9) },
                new Rating { RatingId = 16, UserId = 4, BookId = 6, RatingValue = 5, Review = "Cuốn sách này đã truyền cảm hứng cho tôi theo đuổi ước mơ của mình. Tuyệt vời!", CreatedDate = seedDate.AddDays(-7), UpdatedDate = seedDate.AddDays(-7) },
                
                // Đắc nhân tâm ratings
                new Rating { RatingId = 17, UserId = 3, BookId = 7, RatingValue = 5, Review = "Kinh điển về kỹ năng giao tiếp! Các nguyên tắc vẫn áp dụng được sau gần 100 năm.", CreatedDate = seedDate.AddDays(-6), UpdatedDate = seedDate.AddDays(-6) },
                new Rating { RatingId = 18, UserId = 4, BookId = 7, RatingValue = 4, Review = "Những bài học thực tế về cách ứng xử và làm việc với mọi người. Rất hữu ích.", CreatedDate = seedDate.AddDays(-5), UpdatedDate = seedDate.AddDays(-5) },
                new Rating { RatingId = 19, UserId = 5, BookId = 7, RatingValue = 5, Review = "Dale Carnegie thực sự hiểu tâm lý con người. Mỗi chương đều có giá trị ứng dụng cao.", CreatedDate = seedDate.AddDays(-3), UpdatedDate = seedDate.AddDays(-3) }
            );
        }
    }
}
