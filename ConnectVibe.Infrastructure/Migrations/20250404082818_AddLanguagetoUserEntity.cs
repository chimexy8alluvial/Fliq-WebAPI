using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguagetoUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "AuditTrails",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HttpMethod",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "Endpoint",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "HttpMethod",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Users");
        }
    }
}
