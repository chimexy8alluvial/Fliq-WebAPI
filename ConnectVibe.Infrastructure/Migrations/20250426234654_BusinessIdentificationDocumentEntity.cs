using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BusinessIdentificationDocumentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "BusinessIdentificationDocuments",
               columns: table => new
               {
                   Id = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                   DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                   IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                   BusinessIdentificationDocumentTypeId = table.Column<int>(type: "int", nullable: false),
                   FrontDocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   BackDocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),                 
                   IsVerified = table.Column<bool>(type: "bit", nullable: false),
                   VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_BusinessIdentificationDocuments", x => x.Id);
                   table.ForeignKey(
                       name: "FK_BusinessIdentificationDocuments_BusinessIdentificationDocumentTypes_BusinessIdentificationDocumentTypeId",
                       column: x => x.BusinessIdentificationDocumentTypeId,
                       principalTable: "BusinessIdentificationDocumentTypes",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Restrict);
               });

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

            migrationBuilder.DropTable(name: "BusinessIdentificationDocuments");
        }
    }
}
