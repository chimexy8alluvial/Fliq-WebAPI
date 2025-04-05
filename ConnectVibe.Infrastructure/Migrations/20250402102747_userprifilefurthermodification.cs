using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userprifilefurthermodification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "SexualOrientation");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Religion");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Occupation");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Ethnicity");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "EducationStatus");

            migrationBuilder.AddColumn<bool>(
                name: "IsEducationStatusVisible",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEthnicityVisible",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOccupationVisible",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReligionVisible",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSexualOrientationVisible",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEducationStatusVisible",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsEthnicityVisible",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsOccupationVisible",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsReligionVisible",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsSexualOrientationVisible",
                table: "UserProfiles");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "SexualOrientation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Religion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Occupation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Ethnicity",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "EducationStatus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 3, 31, 18, 13, 9, 788, DateTimeKind.Utc).AddTicks(7394));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 3, 31, 18, 13, 9, 788, DateTimeKind.Utc).AddTicks(7397));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 3, 31, 18, 13, 9, 788, DateTimeKind.Utc).AddTicks(7398));
        }
    }
}
