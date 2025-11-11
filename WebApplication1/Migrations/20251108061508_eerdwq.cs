using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class eerdwq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CachedAt",
                table: "CurrencyHistoryCache");

            migrationBuilder.RenameColumn(
                name: "JsonData",
                table: "CurrencyHistoryCache",
                newName: "DataJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataJson",
                table: "CurrencyHistoryCache",
                newName: "JsonData");

            migrationBuilder.AddColumn<DateTime>(
                name: "CachedAt",
                table: "CurrencyHistoryCache",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
