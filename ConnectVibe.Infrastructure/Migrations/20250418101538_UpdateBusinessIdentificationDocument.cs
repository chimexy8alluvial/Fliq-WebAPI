using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBusinessIdentificationDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BusinessIdentificationDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessIdentificationDocuments_UserId",
                table: "BusinessIdentificationDocuments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessIdentificationDocuments_Users_UserId",
                table: "BusinessIdentificationDocuments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessIdentificationDocuments_Users_UserId",
                table: "BusinessIdentificationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_BusinessIdentificationDocuments_UserId",
                table: "BusinessIdentificationDocuments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BusinessIdentificationDocuments");
        }
    }
}
