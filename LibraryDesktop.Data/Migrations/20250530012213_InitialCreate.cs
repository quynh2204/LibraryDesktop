using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryDesktop.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AvatarUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Coins = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TotalChapters = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<int>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                    table.ForeignKey(
                        name: "FK_Books_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethod = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    QrCodeData = table.Column<string>(type: "TEXT", nullable: true),
                    PaymentToken = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PaymentUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ThemeMode = table.Column<int>(type: "INTEGER", nullable: false),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.SettingId);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    ChapterId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChapterNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ChapterTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    GitHubContentUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.ChapterId);
                    table.ForeignKey(
                        name: "FK_Chapters_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    RatingValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Review = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_Ratings_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    FavoriteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.FavoriteId);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedDate", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "Giả Tưởng", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Truyện và tiểu thuyết giả tưởng", true },
                    { 2, "Văn học hiện đại", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Truyện và tác phẩm văn học hiện đại", true },
                    { 3, "Kinh tế", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sách và truyện về lĩnh vực kinh tế", true },
                    { 4, "Trinh thám", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Truyện và tiểu thuyết trinh thám", true },
                    { 5, "Lịch sử", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sách và truyện về lịch sử", true },
                    { 6, "Triết học", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tiểu thuyết và sách về triết học", true },
                    { 7, "Kỹ năng sống", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sách về kỹ năng sống", true },
                    { 8, "Khoa học viễn tưởng", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Truyện và tiểu thuyết khoa học viễn tưởng", true }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AvatarUrl", "Coins", "Email", "PasswordHash", "RegistrationDate", "Username" },
                values: new object[,]
                {
                    { 1, null, 100, "demo@library.com", "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "demo" },
                    { 2, null, 150, "alice@library.com", "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", new DateTime(2024, 12, 2, 0, 0, 0, 0, DateTimeKind.Utc), "alice" },
                    { 3, null, 80, "bob@library.com", "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Utc), "bob" },
                    { 4, null, 200, "carol@library.com", "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", new DateTime(2024, 12, 17, 0, 0, 0, 0, DateTimeKind.Utc), "carol" },
                    { 5, null, 120, "david@library.com", "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Utc), "david" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Author", "CategoryId", "CoverImageUrl", "CreatedDate", "Description", "Price", "Status", "Title", "TotalChapters", "ViewCount" },
                values: new object[,]
                {
                    { 1, "J.K. Rowling", 1, "", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Câu chuyện về cậu bé phù thủy Harry Potter và cuộc phiêu lưu đầu tiên tại trường Hogwarts. Khám phá thế giới phép thuật đầy kỳ diệu và những người bạn đồng hành.", 0, 2, "Harry Potter và Hòn đá Phù thủy", 0, 2000 },
                    { 2, "Nguyễn Nhật Ánh", 2, "Assets/2ef1ef06a27bf5cd68fea90a24cc96dd.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tác phẩm kể về tuổi thơ của những đứa trẻ miền quê, với những kỷ niệm đẹp về tình anh em, tình làng nghĩa xóm và những bài học cuộc sống quý giá.", 0, 2, "Tôi thấy hoa vàng trên cỏ xanh", 0, 1000 },
                    { 3, "Arthur Conan Doyle", 4, "Assets/3398eb12b32fa930e105e701b708bc9a.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tuyển tập những vụ án kinh điển của thám tử vĩ đại Sherlock Holmes và người bạn đồng hành Watson. Những câu chuyện trinh thám hấp dẫn và đầy bí ẩn.", 0, 1, "Sherlock Holmes: Cuộc phiêu lưu của Sherlock Holmes", 0, 500 },
                    { 4, "Robert Kiyosaki", 3, "Assets/5cb878e981ec841cf8963c2dbfc837c3.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cuốn sách dạy về tư duy tài chính và cách quản lý tiền bạc hiệu quả. Những bài học quý giá về đầu tư và xây dựng tài sản.", 50, 1, "Dạy con làm giàu", 0, 600 },
                    { 5, "Ngô Sĩ Liên", 5, "Assets/65b07f0ccb5631d4025d509c0c14e62d.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tác phẩm sử học quan trọng ghi chép lịch sử Việt Nam từ thời cổ đại đến thế kỷ XV. Nguồn tài liệu quý giá về văn hóa và lịch sử dân tộc.", 0, 2, "Lịch sử Việt Nam: Đại Việt sử ký toàn thư", 0, 700 },
                    { 6, "Paulo Coelho", 6, "Assets/6a81c3d24a73711e02ba8593c067bccf.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Câu chuyện về chàng chăn cừu Santiago và cuộc hành trình tìm kiếm kho báu. Một tác phẩm triết lý sâu sắc về ước mơ và ý nghĩa cuộc sống.", 50, 1, "Nhà giả kim", 0, 800 },
                    { 7, "Dale Carnegie", 7, "Assets/9a321b2c38deed11aa8fb0e879cc6610.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cuốn sách kinh điển về nghệ thuật giao tiếp và ứng xử. Hướng dẫn cách xây dựng mối quan hệ tốt và thành công trong cuộc sống.", 50, 1, "Đắc nhân tâm", 0, 900 }
                });

            migrationBuilder.InsertData(
                table: "UserSettings",
                columns: new[] { "SettingId", "FontSize", "ThemeMode", "UserId" },
                values: new object[] { 1, 12, 0, 1 });

            migrationBuilder.InsertData(
                table: "Chapters",
                columns: new[] { "ChapterId", "BookId", "ChapterNumber", "ChapterTitle", "GitHubContentUrl", "PublishedDate" },
                values: new object[,]
                {
                    { 1, 1, 1, "Đứa bé vẫn sống", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2001.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, 2, "Tấm kính biến mất", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2002.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 1, 3, "Những lá thư không xuất xứ", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2003.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 1, 4, "Người giữ khóa", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2004.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 1, 5, "Hẻm Xéo", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Harry%20Potter%20V%C3%A0%20H%C3%B2n%20%C4%90%C3%A1%20Ph%C3%B9%20Th%E1%BB%A7y/Ch%C6%B0%C6%A1ng%2005.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 2, 1, "Hoa tay", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/T%C3%B4i%20Th%E1%BA%A5y%20Hoa%20V%C3%A0ng%20Tr%C3%AAn%20C%E1%BB%8F%20Xanh/Ch%C6%B0%C6%A1ng%2001.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 2, 2, "Những ngón tay", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/T%C3%B4i%20Th%E1%BA%A5y%20Hoa%20V%C3%A0ng%20Tr%C3%AAn%20C%E1%BB%8F%20Xanh/Ch%C6%B0%C6%A1ng%2002.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 2, 3, "Chú Đàn", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/T%C3%B4i%20Th%E1%BA%A5y%20Hoa%20V%C3%A0ng%20Tr%C3%AAn%20C%E1%BB%8F%20Xanh/Ch%C6%B0%C6%A1ng%2003.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 2, 4, "Chuyện ma của chú Đàn", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/T%C3%B4i%20Th%E1%BA%A5y%20Hoa%20V%C3%A0ng%20Tr%C3%AAn%20C%E1%BB%8F%20Xanh/Ch%C6%B0%C6%A1ng%2004.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 3, 1, "Dải băng lốm đốm", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2001.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 3, 2, "Hội tóc hung", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2002.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 3, 3, "Bí ẩn ở thung lũng", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2003.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 3, 4, "Năm hột cam", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2004.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 3, 5, "Chiếc vương miện", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Cu%E1%BB%99c%20Phi%C3%AAu%20L%C6%B0u%20C%E1%BB%A7a%20Sherlock%20Holmes/Ch%C6%B0%C6%A1ng%2005.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 4, 1, "Cha giàu, cha nghèo", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/D%E1%BA%A1y%20Con%20L%C3%A0m%20Gi%C3%A0u%20-%20T%E1%BA%ADp%201/Ch%C6%B0%C6%A1ng%2001.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 4, 2, "Người giàu không làm việc vì tiền", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/D%E1%BA%A1y%20Con%20L%C3%A0m%20Gi%C3%A0u%20-%20T%E1%BA%ADp%201/Ch%C6%B0%C6%A1ng%2002.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 4, 3, "Tại sao phải dạy con về tài chính?", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/D%E1%BA%A1y%20Con%20L%C3%A0m%20Gi%C3%A0u%20-%20T%E1%BA%ADp%201/Ch%C6%B0%C6%A1ng%2003.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 5, 1, "Đại Việt Sử Ký Ngoại Kỷ Toàn Thư: Quyển I", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/L%E1%BB%8Bch%20S%E1%BB%AD%20Vi%E1%BB%87t%20Nam%20-%20%C4%90%E1%BA%A1i%20Vi%E1%BB%87t%20S%E1%BB%AD%20K%C3%BD%20To%C3%A0n%20Th%C6%B0/Quy%E1%BB%83n%20I.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 5, 2, "Đại Việt Sử Ký Ngoại Kỷ Toàn Thư: Quyển II", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/L%E1%BB%8Bch%20S%E1%BB%AD%20Vi%E1%BB%87t%20Nam%20-%20%C4%90%E1%BA%A1i%20Vi%E1%BB%87t%20S%E1%BB%AD%20K%C3%BD%20To%C3%A0n%20Th%C6%B0/Quy%E1%BB%83n%20II.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 5, 3, "Đại Việt Sử Ký Ngoại Kỷ Toàn Thư: Quyển III", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/L%E1%BB%8Bch%20S%E1%BB%AD%20Vi%E1%BB%87t%20Nam%20-%20%C4%90%E1%BA%A1i%20Vi%E1%BB%87t%20S%E1%BB%AD%20K%C3%BD%20To%C3%A0n%20Th%C6%B0/Quy%E1%BB%83n%20III.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 6, 1, "Chương 1", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Nh%C3%A0%20Gi%E1%BA%A3%20Kim/Ch%C6%B0%C6%A1ng%2001.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 6, 2, "Chương 2", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Nh%C3%A0%20Gi%E1%BA%A3%20Kim/Ch%C6%B0%C6%A1ng%2002.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 6, 3, "Chương 3", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Nh%C3%A0%20Gi%E1%BA%A3%20Kim/Ch%C6%B0%C6%A1ng%2003.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 6, 4, "Chương 4", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/Nh%C3%A0%20Gi%E1%BA%A3%20Kim/Ch%C6%B0%C6%A1ng%2004.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 7, 1, "Muốn lấy mật thì đừng phá tổ ong", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/%C4%90%E1%BA%AFc%20Nh%C3%A2n%20T%C3%A2m/Ch%C6%B0%C6%A1ng%2001.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, 7, 2, "Bí mật lớn nhất trong phép ứng xử", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/%C4%90%E1%BA%AFc%20Nh%C3%A2n%20T%C3%A2m/Ch%C6%B0%C6%A1ng%2002.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, 7, 3, "Ai làm được điều dưới đây", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/%C4%90%E1%BA%AFc%20Nh%C3%A2n%20T%C3%A2m/Ch%C6%B0%C6%A1ng%2003.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, 7, 4, "Thành thật quan tâm đến người khác", "https://raw.githubusercontent.com/PeanLutHuynh/Project_Library-Books/master/%C4%90%E1%BA%AFc%20Nh%C3%A2n%20T%C3%A2m/Ch%C6%B0%C6%A1ng%2004.txt", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "RatingId", "BookId", "CreatedDate", "RatingValue", "Review", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 12, 7, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Cuốn sách tuyệt vời! Câu chuyện phù thủy rất hấp dẫn và đầy màu sắc. Tôi đã đọc nhiều lần và vẫn thích.", new DateTime(2024, 12, 7, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 2, 1, new DateTime(2024, 12, 10, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Hay nhưng hơi dài dòng ở một số đoạn. Nhìn chung vẫn là một tác phẩm đáng đọc.", new DateTime(2024, 12, 10, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 3, 1, new DateTime(2024, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Thế giới phép thuật được xây dựng rất chi tiết và logic. J.K. Rowling thực sự là một thiên tài!", new DateTime(2024, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 4, 2, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Nguyễn Nhật Ánh viết về tuổi thơ rất chân thực và cảm động. Đọc xong như quay về thời thơ ấu.", new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 5, 2, new DateTime(2024, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Câu chuyện đẹp về tình anh em và làng quê Việt Nam. Có những đoạn khiến tôi rơi nước mắt.", new DateTime(2024, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 6, 3, new DateTime(2024, 12, 13, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Trinh thám kinh điển! Sherlock Holmes thông minh và các vụ án rất logic. Không thể bỏ xuống được.", new DateTime(2024, 12, 13, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 7, 3, new DateTime(2024, 12, 18, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Phong cách viết của Conan Doyle rất hấp dẫn. Mỗi câu chuyện đều có twist bất ngờ.", new DateTime(2024, 12, 18, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 8, 3, new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Đây là lý do tại sao Sherlock Holmes trở thành biểu tượng thám tử. Xuất sắc!", new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 9, 4, new DateTime(2024, 12, 15, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Cuốn sách thay đổi tư duy của tôi về tiền bạc và đầu tư. Rất thực tế và dễ hiểu.", new DateTime(2024, 12, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 10, 4, new DateTime(2024, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Có nhiều ý hay nhưng một số quan điểm hơi Mỹ hóa. Cần điều chỉnh cho phù hợp Việt Nam.", new DateTime(2024, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 11, 4, new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Bài học về tài chính cá nhân rất bổ ích. Đáng đọc cho mọi lứa tuổi.", new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 12, 5, new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Tài liệu lịch sử quý giá, giúp hiểu rõ hơn về nguồn gốc dân tộc. Hơi khó đọc với người hiện đại.", new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 13, 5, new DateTime(2024, 12, 24, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Đại Việt sử ký là kho tàng văn hóa dân tộc. Mọi người Việt Nam nên đọc ít nhất một lần.", new DateTime(2024, 12, 24, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 14, 6, new DateTime(2024, 12, 17, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Triết lý sâu sắc về cuộc sống và ước mơ. Paulo Coelho viết rất hay và ý nghĩa.", new DateTime(2024, 12, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 15, 6, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Câu chuyện đơn giản nhưng chứa đựng nhiều bài học nhân sinh. Đáng suy ngẫm.", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 16, 6, new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Cuốn sách này đã truyền cảm hứng cho tôi theo đuổi ước mơ của mình. Tuyệt vời!", new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 17, 7, new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Kinh điển về kỹ năng giao tiếp! Các nguyên tắc vẫn áp dụng được sau gần 100 năm.", new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 18, 7, new DateTime(2024, 12, 27, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Những bài học thực tế về cách ứng xử và làm việc với mọi người. Rất hữu ích.", new DateTime(2024, 12, 27, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 19, 7, new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Dale Carnegie thực sự hiểu tâm lý con người. Mỗi chương đều có giá trị ứng dụng cao.", new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Utc), 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_BookId_ChapterNumber",
                table: "Chapters",
                columns: new[] { "BookId", "ChapterNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentToken",
                table: "Payments",
                column: "PaymentToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_BookId",
                table: "Ratings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId_BookId",
                table: "Ratings",
                columns: new[] { "UserId", "BookId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_BookId",
                table: "UserFavorites",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId_BookId",
                table: "UserFavorites",
                columns: new[] { "UserId", "BookId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
