using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryDesktop.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
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
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
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
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
                    { 1, "Fantasy", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fantasy stories and novels", true },
                    { 2, "Romance", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Romance stories and novels", true },
                    { 3, "Sci-Fi", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Science fiction stories", true },
                    { 4, "Mystery", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mystery and thriller stories", true },
                    { 5, "Adventure", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Adventure stories", true }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AvatarUrl", "Email", "PasswordHash", "RegistrationDate", "Username" },
                values: new object[] { 1, null, "demo@library.com", "Z4m0WAouR0CZpMn4ZqNX0nnr8+bfEkfV7J0Ps7umRjE=", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "demo" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Author", "CategoryId", "CoverImageUrl", "CreatedDate", "Description", "Price", "Status", "Title", "TotalChapters", "ViewCount" },
                values: new object[,]
                {
                    { 1, "Elena Moonstone", 1, "Assets/0d080b47aaa3ab11160e60091f5ecbb7.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magical journey through an ancient forest filled with mystical creatures and forgotten secrets.", 15.99m, 1, "The Enchanted Forest", 0, 0 },
                    { 2, "Sarah Martinez", 2, "Assets/2ef1ef06a27bf5cd68fea90a24cc96dd.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A heartwarming romance set in the bustling streets of New York City.", 12.99m, 1, "Love in the City", 0, 0 },
                    { 3, "Dr. Michael Chen", 3, "Assets/3398eb12b32fa930e105e701b708bc9a.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "An epic science fiction adventure exploring the boundaries of space and time.", 18.99m, 1, "Quantum Horizons", 0, 0 },
                    { 4, "James Blackwood", 4, "Assets/5cb878e981ec841cf8963c2dbfc837c3.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A gripping mystery thriller following Detective Morgan through the darkest corners of the city.", 14.99m, 1, "The Shadow Detective", 0, 0 },
                    { 5, "Adventure Kelly", 5, "Assets/65b07f0ccb5631d4025d509c0c14e62d.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "An thrilling adventure story of climbing the world's most dangerous peaks.", 16.99m, 1, "Mountain Quest", 0, 0 },
                    { 6, "Aria Dragonheart", 1, "Assets/6a81c3d24a73711e02ba8593c067bccf.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The epic tale of the last dragon rider and their quest to save the realm.", 19.99m, 1, "Dragon's Legacy", 0, 0 },
                    { 7, "Luna Starfield", 2, "Assets/9a321b2c38deed11aa8fb0e879cc6610.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A cosmic romance spanning galaxies and defying the laws of physics.", 13.99m, 1, "Starbound Lovers", 0, 0 },
                    { 8, "Prof. Alexandra Time", 3, "Assets/c879fb508a3217ace62142bf6f7b72c1.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A mind-bending sci-fi thriller about time travel and its consequences.", 17.99m, 1, "Time Paradox", 0, 0 },
                    { 9, "Rebecca Stone", 4, "Assets/e19d44be068aeef341ef687ce43ce5a3.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "An ancient code holds the key to preventing a global catastrophe.", 15.99m, 1, "The Lost Cipher", 0, 0 },
                    { 10, "Captain Marina Blue", 5, "Assets/e3c33ed5f3d5d99567ae20bd138aa913.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dive into the deepest mysteries of the ocean in this underwater adventure.", 14.99m, 1, "Ocean Explorer", 0, 0 },
                    { 11, "Merlin Wiseheart", 1, "Assets/f0e51f4a153d25e0438d429892ac8fa6.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A young apprentice discovers hidden powers and ancient magical secrets.", 16.99m, 1, "The Wizard's Apprentice", 0, 0 },
                    { 12, "Scarlett Dreams", 2, "Assets/f4c232745d79b53ac510d102a347fb4b.jpg", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A passionate love story that blooms under the moonlit sky.", 11.99m, 1, "Midnight Romance", 0, 0 }
                });

            migrationBuilder.InsertData(
                table: "UserSettings",
                columns: new[] { "SettingId", "Balance", "FontSize", "ThemeMode", "UserId" },
                values: new object[] { 1, 100.00m, 12, 0, 1 });

            migrationBuilder.InsertData(
                table: "Chapters",
                columns: new[] { "ChapterId", "BookId", "ChapterNumber", "ChapterTitle", "GitHubContentUrl", "PublishedDate" },
                values: new object[,]
                {
                    { 1, 1, 1, "The Mysterious Path", "https://github.com/example/enchanted-forest/chapter1.md", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, 2, "Meeting the Guardian", "https://github.com/example/enchanted-forest/chapter2.md", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 2, 1, "First Encounter", "https://github.com/example/love-city/chapter1.md", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 3, 1, "The Quantum Discovery", "https://github.com/example/quantum-horizons/chapter1.md", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
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
                column: "UserId",
                unique: true);
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
