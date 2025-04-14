using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewContentAndComplianceSchemaAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "SpeedDatingEvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "SpeedDatingEvents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentCreationStatus",
                table: "SpeedDatingEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "SpeedDatingEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CreatorIsAdmin",
                table: "SpeedDatingEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "SpeedDatingEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "SpeedDatingEvents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "PromptQuestions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "PromptQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentCreationStatus",
                table: "PromptQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "PromptQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CreatorIsAdmin",
                table: "PromptQuestions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "PromptQuestions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "PromptQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisconnectionResolutionOption",
                table: "GameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WinnerId",
                table: "GameSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Games",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentCreationStatus",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameDisconnectionResolutionOption",
                table: "GameRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Events",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentCreationStatus",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CreatorIsAdmin",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "BlindDates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "BlindDates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentCreationStatus",
                table: "BlindDates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CreatorIsAdmin",
                table: "BlindDates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "BlindDates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "BlindDates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComplianceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Compliances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComplianceTypeId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VersionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compliances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compliances_ComplianceTypes_ComplianceTypeId",
                        column: x => x.ComplianceTypeId,
                        principalTable: "ComplianceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserConsentActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptIn = table.Column<bool>(type: "bit", nullable: false),
                    ComplianceId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConsentActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConsentActions_Compliances_ComplianceId",
                        column: x => x.ComplianceId,
                        principalTable: "Compliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserConsentActions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpeedDatingEvents_CreatedByUserId",
                table: "SpeedDatingEvents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptQuestions_CreatedByUserId",
                table: "PromptQuestions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Compliances_ComplianceTypeId",
                table: "Compliances",
                column: "ComplianceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConsentActions_ComplianceId",
                table: "UserConsentActions",
                column: "ComplianceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConsentActions_UserId",
                table: "UserConsentActions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromptQuestions_Users_CreatedByUserId",
                table: "PromptQuestions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpeedDatingEvents_Users_CreatedByUserId",
                table: "SpeedDatingEvents",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_PromptQuestions_Users_CreatedByUserId",
                table: "PromptQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_SpeedDatingEvents_Users_CreatedByUserId",
                table: "SpeedDatingEvents");

            migrationBuilder.DropTable(
                name: "UserConsentActions");

            migrationBuilder.DropTable(
                name: "Compliances");

            migrationBuilder.DropTable(
                name: "ComplianceTypes");

            migrationBuilder.DropIndex(
                name: "IX_SpeedDatingEvents_CreatedByUserId",
                table: "SpeedDatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_PromptQuestions_CreatedByUserId",
                table: "PromptQuestions");

            migrationBuilder.DropIndex(
                name: "IX_Events_UserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "SpeedDatingEvents");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "SpeedDatingEvents");

            migrationBuilder.DropColumn(
                name: "ContentCreationStatus",
                table: "SpeedDatingEvents");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SpeedDatingEvents");

            migrationBuilder.DropColumn(
                name: "CreatorIsAdmin",
                table: "SpeedDatingEvents");

            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "SpeedDatingEvents");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "SpeedDatingEvents");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "PromptQuestions");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "PromptQuestions");

            migrationBuilder.DropColumn(
                name: "ContentCreationStatus",
                table: "PromptQuestions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PromptQuestions");

            migrationBuilder.DropColumn(
                name: "CreatorIsAdmin",
                table: "PromptQuestions");

            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "PromptQuestions");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "PromptQuestions");

            migrationBuilder.DropColumn(
                name: "DisconnectionResolutionOption",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ContentCreationStatus",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameDisconnectionResolutionOption",
                table: "GameRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ContentCreationStatus",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatorIsAdmin",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "BlindDates");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "BlindDates");

            migrationBuilder.DropColumn(
                name: "ContentCreationStatus",
                table: "BlindDates");

            migrationBuilder.DropColumn(
                name: "CreatorIsAdmin",
                table: "BlindDates");

            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "BlindDates");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "BlindDates");

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
