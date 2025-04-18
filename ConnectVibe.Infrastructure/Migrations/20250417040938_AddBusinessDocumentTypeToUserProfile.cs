using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessDocumentTypeToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessDocumentTypeId",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_BusinessDocumentTypeId",
                table: "UserProfiles",
                column: "BusinessDocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_BusinessDocumentTypes_BusinessDocumentTypeId",
                table: "UserProfiles",
                column: "BusinessDocumentTypeId",
                principalTable: "BusinessDocumentTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_BusinessDocumentTypes_BusinessDocumentTypeId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_BusinessDocumentTypeId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BusinessDocumentTypeId",
                table: "UserProfiles");
        }
    }
}
