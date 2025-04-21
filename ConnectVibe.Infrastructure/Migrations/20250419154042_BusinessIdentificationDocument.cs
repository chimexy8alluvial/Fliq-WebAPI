using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BusinessIdentificationDocument : Migration
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BusinessIdentificationDocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    FrontDocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackDocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessIdentificationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessIdentificationDocuments_BusinessIdentificationDocumentTypes_BusinessIdentificationDocumentTypeId",
                        column: x => x.BusinessIdentificationDocumentTypeId,
                        principalTable: "BusinessIdentificationDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessIdentificationDocuments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessIdentificationDocuments_BusinessIdentificationDocumentTypeId",
                table: "BusinessIdentificationDocuments",
                column: "BusinessIdentificationDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessIdentificationDocuments_UserId",
                table: "BusinessIdentificationDocuments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessIdentificationDocuments");
        }
    }
}
