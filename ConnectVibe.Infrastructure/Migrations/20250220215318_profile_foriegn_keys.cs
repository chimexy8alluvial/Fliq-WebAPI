using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class profile_foriegn_keys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_EducationStatus_EducationStatusId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Ethnicity_EthnicityId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Gender_GenderId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Occupation_OccupationId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Religion_ReligionId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_SexualOrientation_SexualOrientationId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_EducationStatusId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_EthnicityId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_GenderId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_OccupationId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_ReligionId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_SexualOrientationId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_WantKidsId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "EducationStatusId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "EthnicityId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "OccupationId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ReligionId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "SexualOrientationId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "WantKidsId",
                table: "UserProfiles");

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "WantKids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "SexualOrientation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "Religion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "Occupation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "HaveKids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "Gender",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "Ethnicity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "EducationStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 21, 53, 15, 201, DateTimeKind.Utc).AddTicks(6656));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 21, 53, 15, 201, DateTimeKind.Utc).AddTicks(6667));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 21, 53, 15, 201, DateTimeKind.Utc).AddTicks(6672));

            migrationBuilder.CreateIndex(
                name: "IX_WantKids_UserProfileId",
                table: "WantKids",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SexualOrientation_UserProfileId",
                table: "SexualOrientation",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Religion_UserProfileId",
                table: "Religion",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Occupation_UserProfileId",
                table: "Occupation",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HaveKids_UserProfileId",
                table: "HaveKids",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gender_UserProfileId",
                table: "Gender",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ethnicity_UserProfileId",
                table: "Ethnicity",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationStatus_UserProfileId",
                table: "EducationStatus",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EducationStatus_UserProfiles_UserProfileId",
                table: "EducationStatus",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ethnicity_UserProfiles_UserProfileId",
                table: "Ethnicity",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gender_UserProfiles_UserProfileId",
                table: "Gender",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HaveKids_UserProfiles_UserProfileId",
                table: "HaveKids",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Occupation_UserProfiles_UserProfileId",
                table: "Occupation",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Religion_UserProfiles_UserProfileId",
                table: "Religion",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SexualOrientation_UserProfiles_UserProfileId",
                table: "SexualOrientation",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WantKids_UserProfiles_UserProfileId",
                table: "WantKids",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationStatus_UserProfiles_UserProfileId",
                table: "EducationStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_Ethnicity_UserProfiles_UserProfileId",
                table: "Ethnicity");

            migrationBuilder.DropForeignKey(
                name: "FK_Gender_UserProfiles_UserProfileId",
                table: "Gender");

            migrationBuilder.DropForeignKey(
                name: "FK_HaveKids_UserProfiles_UserProfileId",
                table: "HaveKids");

            migrationBuilder.DropForeignKey(
                name: "FK_Occupation_UserProfiles_UserProfileId",
                table: "Occupation");

            migrationBuilder.DropForeignKey(
                name: "FK_Religion_UserProfiles_UserProfileId",
                table: "Religion");

            migrationBuilder.DropForeignKey(
                name: "FK_SexualOrientation_UserProfiles_UserProfileId",
                table: "SexualOrientation");

            migrationBuilder.DropForeignKey(
                name: "FK_WantKids_UserProfiles_UserProfileId",
                table: "WantKids");

            migrationBuilder.DropIndex(
                name: "IX_WantKids_UserProfileId",
                table: "WantKids");

            migrationBuilder.DropIndex(
                name: "IX_SexualOrientation_UserProfileId",
                table: "SexualOrientation");

            migrationBuilder.DropIndex(
                name: "IX_Religion_UserProfileId",
                table: "Religion");

            migrationBuilder.DropIndex(
                name: "IX_Occupation_UserProfileId",
                table: "Occupation");

            migrationBuilder.DropIndex(
                name: "IX_HaveKids_UserProfileId",
                table: "HaveKids");

            migrationBuilder.DropIndex(
                name: "IX_Gender_UserProfileId",
                table: "Gender");

            migrationBuilder.DropIndex(
                name: "IX_Ethnicity_UserProfileId",
                table: "Ethnicity");

            migrationBuilder.DropIndex(
                name: "IX_EducationStatus_UserProfileId",
                table: "EducationStatus");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "WantKids");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "SexualOrientation");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Religion");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Occupation");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "HaveKids");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Gender");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Ethnicity");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "EducationStatus");

            migrationBuilder.AddColumn<int>(
                name: "EducationStatusId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EthnicityId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GenderId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HaveKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OccupationId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReligionId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SexualOrientationId",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WantKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 21, 38, 6, 701, DateTimeKind.Utc).AddTicks(1610));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 21, 38, 6, 701, DateTimeKind.Utc).AddTicks(1616));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 21, 38, 6, 701, DateTimeKind.Utc).AddTicks(1616));

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_EducationStatusId",
                table: "UserProfiles",
                column: "EducationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_EthnicityId",
                table: "UserProfiles",
                column: "EthnicityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_GenderId",
                table: "UserProfiles",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_HaveKidsId",
                table: "UserProfiles",
                column: "HaveKidsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_OccupationId",
                table: "UserProfiles",
                column: "OccupationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ReligionId",
                table: "UserProfiles",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_SexualOrientationId",
                table: "UserProfiles",
                column: "SexualOrientationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_WantKidsId",
                table: "UserProfiles",
                column: "WantKidsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_EducationStatus_EducationStatusId",
                table: "UserProfiles",
                column: "EducationStatusId",
                principalTable: "EducationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Ethnicity_EthnicityId",
                table: "UserProfiles",
                column: "EthnicityId",
                principalTable: "Ethnicity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Gender_GenderId",
                table: "UserProfiles",
                column: "GenderId",
                principalTable: "Gender",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles",
                column: "HaveKidsId",
                principalTable: "HaveKids",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Occupation_OccupationId",
                table: "UserProfiles",
                column: "OccupationId",
                principalTable: "Occupation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Religion_ReligionId",
                table: "UserProfiles",
                column: "ReligionId",
                principalTable: "Religion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_SexualOrientation_SexualOrientationId",
                table: "UserProfiles",
                column: "SexualOrientationId",
                principalTable: "SexualOrientation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles",
                column: "WantKidsId",
                principalTable: "WantKids",
                principalColumn: "Id");
        }
    }
}
