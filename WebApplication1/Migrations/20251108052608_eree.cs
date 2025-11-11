using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class eree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "CurrencyHistoryCache",
                newName: "JsonData");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CurrencyHistoryCache",
                newName: "CachedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JsonData",
                table: "CurrencyHistoryCache",
                newName: "Data");

            migrationBuilder.RenameColumn(
                name: "CachedAt",
                table: "CurrencyHistoryCache",
                newName: "CreatedAt");
        }
    }
}
