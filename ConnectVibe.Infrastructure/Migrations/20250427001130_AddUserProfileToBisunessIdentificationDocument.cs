using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileToBisunessIdentificationDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "BusinessIdentificationDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessIdentificationDocuments_UserProfileId",
                table: "BusinessIdentificationDocuments",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessIdentificationDocuments_UserProfiles_UserProfileId",
                table: "BusinessIdentificationDocuments",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessIdentificationDocuments_UserProfiles_UserProfileId",
                table: "BusinessIdentificationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_BusinessIdentificationDocuments_UserProfileId",
                table: "BusinessIdentificationDocuments");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "BusinessIdentificationDocuments");

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
    }
}
