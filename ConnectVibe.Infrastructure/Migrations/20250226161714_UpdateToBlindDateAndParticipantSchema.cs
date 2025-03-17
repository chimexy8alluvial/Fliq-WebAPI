using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToBlindDateAndParticipantSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreator",
                table: "BlindDatesParticipants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "BlindDates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 2, 26, 16, 17, 12, 549, DateTimeKind.Utc).AddTicks(45));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 2, 26, 16, 17, 12, 549, DateTimeKind.Utc).AddTicks(48));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 2, 26, 16, 17, 12, 549, DateTimeKind.Utc).AddTicks(49));

            migrationBuilder.CreateIndex(
                name: "IX_BlindDates_CreatedByUserId",
                table: "BlindDates",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlindDates_Users_CreatedByUserId",
                table: "BlindDates",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlindDates_Users_CreatedByUserId",
                table: "BlindDates");

            migrationBuilder.DropIndex(
                name: "IX_BlindDates_CreatedByUserId",
                table: "BlindDates");

            migrationBuilder.DropColumn(
                name: "IsCreator",
                table: "BlindDatesParticipants");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BlindDates");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 2, 26, 4, 14, 16, 750, DateTimeKind.Utc).AddTicks(6538));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 2, 26, 4, 14, 16, 750, DateTimeKind.Utc).AddTicks(6542));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 2, 26, 4, 14, 16, 750, DateTimeKind.Utc).AddTicks(6543));
        }
    }
}
