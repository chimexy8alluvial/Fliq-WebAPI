using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfileWithBusinessIdentificationDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessIdentificationDocumentId",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_BusinessIdentificationDocumentId",
                table: "UserProfiles",
                column: "BusinessIdentificationDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_BusinessIdentificationDocuments_BusinessIdentificationDocumentId",
                table: "UserProfiles",
                column: "BusinessIdentificationDocumentId",
                principalTable: "BusinessIdentificationDocuments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_BusinessIdentificationDocuments_BusinessIdentificationDocumentId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_BusinessIdentificationDocumentId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BusinessIdentificationDocumentId",
                table: "UserProfiles");
        }
    }
}
