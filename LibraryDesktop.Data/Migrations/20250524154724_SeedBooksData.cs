using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryDesktop.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedBooksData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                table: "Chapters",
                columns: new[] { "ChapterId", "BookId", "ChapterNumber", "ChapterTitle", "GitHubContentUrl", "PublishedDate" },
                values: new object[,]
                {
                    { 1, 1, 1, "The Mysterious Path", "https://github.com/example/enchanted-forest/chapter1.md", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, 2, "Meeting the Guardian", "https://github.com/example/enchanted-forest/chapter2.md", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 2, 1, "First Encounter", "https://github.com/example/love-city/chapter1.md", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 3, 1, "The Quantum Discovery", "https://github.com/example/quantum-horizons/chapter1.md", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "ChapterId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "ChapterId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "ChapterId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "ChapterId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 3);
        }
    }
}
