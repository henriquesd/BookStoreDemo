using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "CategoryId", "Description", "Name", "PublishDate", "Value" },
                values: new object[,]
                {
                    { 1, "Author 1", 1, "Test 1", "Book 1", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 2, "Author 2", 1, "Test 2", "Book 2", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 3, "Author 3", 1, "Test 3", "Book 3", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 4, "Author 4", 1, "Test 4", "Book 4", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 5, "Author 5", 1, "Test 5", "Book 5", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 6, "Author 6", 1, "Test 6", "Book 6", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 7, "Author 7", 1, "Test 7", "Book 7", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 8, "Author 8", 1, "Test 8", "Book 8", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 9, "Author 9", 1, "Test 9", "Book 9", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 10, "Author 10", 1, "Test 10", "Book 10", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 11, "Author 11", 1, "Test 11", "Book 11", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 12, "Author 12", 1, "Test 12", "Book 12", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 13, "Author 13", 1, "Test 13", "Book 13", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 14, "Author 14", 1, "Test 14", "Book 14", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 15, "Author 15", 1, "Test 15", "Book 15", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 16, "Author 16", 1, "Test 16", "Book 16", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 17, "Author 17", 1, "Test 17", "Book 17", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 18, "Author 18", 1, "Test 18", "Book 18", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 19, "Author 19", 1, "Test 19", "Book 19", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 20, "Author 20", 1, "Test 20", "Book 20", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 21, "Author 21", 1, "Test 21", "Book 21", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 22, "Author 22", 1, "Test 22", "Book 22", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 23, "Author 23", 1, "Test 23", "Book 23", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 24, "Author 24", 1, "Test 24", "Book 24", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 25, "Author 25", 1, "Test 25", "Book 25", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 26, "Author 26", 1, "Test 26", "Book 26", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 27, "Author 27", 1, "Test 27", "Book 27", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 28, "Author 28", 1, "Test 28", "Book 28", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 29, "Author 29", 1, "Test 29", "Book 29", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 30, "Author 30", 1, "Test 30", "Book 30", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 31, "Author 31", 1, "Test 31", "Book 31", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 32, "Author 32", 1, "Test 32", "Book 32", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 33, "Author 33", 1, "Test 33", "Book 33", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 34, "Author 34", 1, "Test 34", "Book 34", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 35, "Author 35", 1, "Test 35", "Book 35", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 36, "Author 36", 1, "Test 36", "Book 36", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 37, "Author 37", 1, "Test 37", "Book 37", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 38, "Author 38", 1, "Test 38", "Book 38", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 39, "Author 39", 1, "Test 39", "Book 39", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 40, "Author 40", 1, "Test 40", "Book 40", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 41, "Author 41", 1, "Test 41", "Book 41", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 42, "Author 42", 1, "Test 42", "Book 42", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 43, "Author 43", 1, "Test 43", "Book 43", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 44, "Author 44", 1, "Test 44", "Book 44", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 45, "Author 45", 1, "Test 45", "Book 45", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 46, "Author 46", 1, "Test 46", "Book 46", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 47, "Author 47", 1, "Test 47", "Book 47", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 48, "Author 48", 1, "Test 48", "Book 48", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 49, "Author 49", 1, "Test 49", "Book 49", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 50, "Author 50", 1, "Test 50", "Book 50", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 51, "Author 51", 1, "Test 51", "Book 51", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 52, "Author 52", 1, "Test 52", "Book 52", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 53, "Author 53", 1, "Test 53", "Book 53", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 54, "Author 54", 1, "Test 54", "Book 54", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 55, "Author 55", 1, "Test 55", "Book 55", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 56, "Author 56", 1, "Test 56", "Book 56", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 57, "Author 57", 1, "Test 57", "Book 57", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 58, "Author 58", 1, "Test 58", "Book 58", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 59, "Author 59", 1, "Test 59", "Book 59", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 60, "Author 60", 1, "Test 60", "Book 60", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 61, "Author 61", 1, "Test 61", "Book 61", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 62, "Author 62", 1, "Test 62", "Book 62", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 63, "Author 63", 1, "Test 63", "Book 63", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 64, "Author 64", 1, "Test 64", "Book 64", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 65, "Author 65", 1, "Test 65", "Book 65", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 66, "Author 66", 1, "Test 66", "Book 66", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 67, "Author 67", 1, "Test 67", "Book 67", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 68, "Author 68", 1, "Test 68", "Book 68", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 69, "Author 69", 1, "Test 69", "Book 69", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 70, "Author 70", 1, "Test 70", "Book 70", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 71, "Author 71", 1, "Test 71", "Book 71", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 72, "Author 72", 1, "Test 72", "Book 72", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 73, "Author 73", 1, "Test 73", "Book 73", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 74, "Author 74", 1, "Test 74", "Book 74", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 75, "Author 75", 1, "Test 75", "Book 75", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 76, "Author 76", 1, "Test 76", "Book 76", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 77, "Author 77", 1, "Test 77", "Book 77", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 78, "Author 78", 1, "Test 78", "Book 78", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 79, "Author 79", 1, "Test 79", "Book 79", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 80, "Author 80", 1, "Test 80", "Book 80", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 81, "Author 81", 1, "Test 81", "Book 81", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 82, "Author 82", 1, "Test 82", "Book 82", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 83, "Author 83", 1, "Test 83", "Book 83", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 84, "Author 84", 1, "Test 84", "Book 84", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 85, "Author 85", 1, "Test 85", "Book 85", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 86, "Author 86", 1, "Test 86", "Book 86", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 87, "Author 87", 1, "Test 87", "Book 87", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 88, "Author 88", 1, "Test 88", "Book 88", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 89, "Author 89", 1, "Test 89", "Book 89", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 90, "Author 90", 1, "Test 90", "Book 90", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 91, "Author 91", 1, "Test 91", "Book 91", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 92, "Author 92", 1, "Test 92", "Book 92", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 93, "Author 93", 1, "Test 93", "Book 93", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 94, "Author 94", 1, "Test 94", "Book 94", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 95, "Author 95", 1, "Test 95", "Book 95", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 96, "Author 96", 1, "Test 96", "Book 96", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 97, "Author 97", 1, "Test 97", "Book 97", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 98, "Author 98", 1, "Test 98", "Book 98", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 99, "Author 99", 1, "Test 99", "Book 99", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 100, "Author 100", 1, "Test 100", "Book 100", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 101, "Author 101", 1, "Test 101", "Book 101", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 102, "Author 102", 1, "Test 102", "Book 102", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 103, "Author 103", 1, "Test 103", "Book 103", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 104, "Author 104", 1, "Test 104", "Book 104", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 105, "Author 105", 1, "Test 105", "Book 105", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 106, "Author 106", 1, "Test 106", "Book 106", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 107, "Author 107", 1, "Test 107", "Book 107", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 108, "Author 108", 1, "Test 108", "Book 108", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 109, "Author 109", 1, "Test 109", "Book 109", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 },
                    { 110, "Author 110", 1, "Test 110", "Book 110", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 110);
        }
    }
}
