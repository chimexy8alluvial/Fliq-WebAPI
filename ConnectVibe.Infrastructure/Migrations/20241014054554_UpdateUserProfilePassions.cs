using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfilePassions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
            name: "Passions",
            table: "UserProfiles",
            type: "nvarchar(max)", 
            nullable: true,
            oldClrType: typeof(string));

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
            name: "Passions",
            table: "UserProfiles",
            type: "nvarchar(max)", 
            nullable: true, 
            oldClrType: typeof(string));
        }
    }
}
