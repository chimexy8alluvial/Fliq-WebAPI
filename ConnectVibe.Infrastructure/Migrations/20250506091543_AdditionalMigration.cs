using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Settings_UserId",
                table: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "TicketSales",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Remove duplicate UserId rows, keeping the one with the lowest Id
            migrationBuilder.Sql(@"
        WITH Duplicates AS (
            SELECT 
                Id,
                ROW_NUMBER() OVER (PARTITION BY UserId ORDER BY Id) AS rn
            FROM Settings
        )
        DELETE FROM Settings WHERE Id IN (SELECT Id FROM Duplicates WHERE rn > 1);
    ");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId",
                unique: true);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Settings_UserId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "TicketSales",
                table: "Events");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId");
        }
    }
}
