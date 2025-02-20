using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nullable_profile_fields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Location_LocationId",
                table: "UserProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "UserProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 14, 50, 88, DateTimeKind.Utc).AddTicks(245));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 14, 50, 88, DateTimeKind.Utc).AddTicks(252));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 14, 50, 88, DateTimeKind.Utc).AddTicks(253));

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Location_LocationId",
                table: "UserProfiles",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Location_LocationId",
                table: "UserProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 6, 31, 541, DateTimeKind.Utc).AddTicks(9831));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 6, 31, 541, DateTimeKind.Utc).AddTicks(9839));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 6, 31, 541, DateTimeKind.Utc).AddTicks(9842));

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Location_LocationId",
                table: "UserProfiles",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
