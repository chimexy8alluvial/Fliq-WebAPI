using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteContactInfromationFromSponsoredEventDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactInfromation",
                table: "SponsoredEventDetails",
                newName: "ContactInformation");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 3, 28, 11, 21, 34, 889, DateTimeKind.Utc).AddTicks(6706));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 3, 28, 11, 21, 34, 889, DateTimeKind.Utc).AddTicks(6913));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 3, 28, 11, 21, 34, 889, DateTimeKind.Utc).AddTicks(6916));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactInformation",
                table: "SponsoredEventDetails",
                newName: "ContactInfromation");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 3, 21, 20, 36, 9, 449, DateTimeKind.Utc).AddTicks(6864));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 3, 21, 20, 36, 9, 449, DateTimeKind.Utc).AddTicks(6873));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 3, 21, 20, 36, 9, 449, DateTimeKind.Utc).AddTicks(6877));
        }
    }
}
