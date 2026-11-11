using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryManagementAPI.Migrations;

/// <inheritdoc />
public partial class _20261111115747_SeedRecords : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Authors",
            columns: new[] { "Id", "FirstName", "LastName", "YearOfBirth" },
            values: new object[,]
            {
                { 1, "Jane", "Austen", 1775 },
                { 2, "Mark", "Twain", 1835 },
                { 3, "Virginia", "Woolf", 1882 },
                { 4, "George", "Orwell", 1903 },
                { 5, "Toni", "Morrison", 1931 },
                { 6, "Gabriel", "Marquez", 1927 },
                { 7, "Mary", "Shelley", 1797 },
                { 8, "Leo", "Tolstoy", 1828 },
                { 9, "Agatha", "Christie", 1890 },
                { 10, "James", "Baldwin", 1924 }
            });

        migrationBuilder.InsertData(
            table: "Categories",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                { 1, "test" },
                { 2, "AC" },
                { 3, "Fiber" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "Authors",
            keyColumn: "Id",
            keyValue: 10);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 3);
    }
}
