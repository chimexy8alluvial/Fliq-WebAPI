using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropBusinessEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchRequests_Businesses_BusinessId",
                table: "MatchRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Businesses_BusinessId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Businesses_BusinessId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDeviceTokens_Businesses_BusinessId",
                table: "UserDeviceTokens");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_UserDeviceTokens_BusinessId",
                table: "UserDeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_BusinessId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BusinessId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_MatchRequests_BusinessId",
                table: "MatchRequests");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "UserDeviceTokens");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "MatchRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "UserDeviceTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "MatchRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyBio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Identification = table.Column<int>(type: "int", nullable: false),
                    IdentificationImageBack = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationImageFront = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDocumentVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailValidated = table.Column<bool>(type: "bit", nullable: false),
                    LastActiveAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Businesses_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDeviceTokens_BusinessId",
                table: "UserDeviceTokens",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_BusinessId",
                table: "Subscriptions",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BusinessId",
                table: "Payments",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchRequests_BusinessId",
                table: "MatchRequests",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_RoleId",
                table: "Businesses",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchRequests_Businesses_BusinessId",
                table: "MatchRequests",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Businesses_BusinessId",
                table: "Payments",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Businesses_BusinessId",
                table: "Subscriptions",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDeviceTokens_Businesses_BusinessId",
                table: "UserDeviceTokens",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");
        }
    }
}
