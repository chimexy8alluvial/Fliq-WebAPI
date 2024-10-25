using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfileType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alter the existing ProfileTypes column to change its type to a string for JSON storage
            migrationBuilder.AlterColumn<string>(
                name: "ProfileTypes",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true, 
                oldClrType: typeof(string), 
                oldType: "nvarchar(max)"); 

            // If there is existing data that needs to be transformed, you can handle that here
            // For example, you might want to initialize existing entries to a default JSON structure.
            migrationBuilder.Sql("UPDATE UserProfiles SET ProfileTypes = '[]' WHERE ProfileTypes IS NULL OR ProfileTypes = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the ProfileTypes column to its original state if needed
            migrationBuilder.AlterColumn<string>(
                name: "ProfileTypes",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true, 
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"); 
        }
    }
}
