using Microsoft.EntityFrameworkCore.Migrations;

namespace Fliq.Infrastructure.Migrations
{
    public partial class AddLanguageToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Users",
                nullable: false, 
                defaultValue: 0); 
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Users");
        }
    }
}