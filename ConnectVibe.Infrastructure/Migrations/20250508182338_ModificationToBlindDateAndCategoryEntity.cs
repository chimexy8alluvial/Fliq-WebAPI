using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModificationToBlindDateAndCategoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "BlindDates");

            migrationBuilder.AddColumn<int>(
                name: "BlindDateId",
                table: "BlindDateCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlindDateId",
                table: "BlindDateCategories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "BlindDates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
