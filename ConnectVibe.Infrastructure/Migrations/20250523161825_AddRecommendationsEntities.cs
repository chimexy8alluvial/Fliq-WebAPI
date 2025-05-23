using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecommendationsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "WantKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HaveKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "CachedRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RecommendationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    BlindDateId = table.Column<int>(type: "int", nullable: true),
                    SpeedDatingEventId = table.Column<int>(type: "int", nullable: true),
                    RecommendedUserId = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: false),
                    ComputedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CachedRecommendations_BlindDates_BlindDateId",
                        column: x => x.BlindDateId,
                        principalTable: "BlindDates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CachedRecommendations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CachedRecommendations_SpeedDatingEvents_SpeedDatingEventId",
                        column: x => x.SpeedDatingEventId,
                        principalTable: "SpeedDatingEvents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CachedRecommendations_Users_RecommendedUserId",
                        column: x => x.RecommendedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CachedRecommendations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    BlindDateId = table.Column<int>(type: "int", nullable: true),
                    SpeedDatingEventId = table.Column<int>(type: "int", nullable: true),
                    InteractedWithUserId = table.Column<int>(type: "int", nullable: true),
                    InteractionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InteractionStrength = table.Column<double>(type: "float", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInteractions_BlindDates_BlindDateId",
                        column: x => x.BlindDateId,
                        principalTable: "BlindDates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserInteractions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserInteractions_SpeedDatingEvents_SpeedDatingEventId",
                        column: x => x.SpeedDatingEventId,
                        principalTable: "SpeedDatingEvents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserInteractions_Users_InteractedWithUserId",
                        column: x => x.InteractedWithUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserInteractions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CachedRecommendations_BlindDateId",
                table: "CachedRecommendations",
                column: "BlindDateId");

            migrationBuilder.CreateIndex(
                name: "IX_CachedRecommendations_EventId",
                table: "CachedRecommendations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_CachedRecommendations_RecommendedUserId",
                table: "CachedRecommendations",
                column: "RecommendedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CachedRecommendations_SpeedDatingEventId",
                table: "CachedRecommendations",
                column: "SpeedDatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_CachedRecommendations_UserId",
                table: "CachedRecommendations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_BlindDateId",
                table: "UserInteractions",
                column: "BlindDateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_EventId",
                table: "UserInteractions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_InteractedWithUserId",
                table: "UserInteractions",
                column: "InteractedWithUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_SpeedDatingEventId",
                table: "UserInteractions",
                column: "SpeedDatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_UserId",
                table: "UserInteractions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles",
                column: "HaveKidsId",
                principalTable: "HaveKids",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles",
                column: "WantKidsId",
                principalTable: "WantKids",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "CachedRecommendations");

            migrationBuilder.DropTable(
                name: "UserInteractions");

            migrationBuilder.AlterColumn<int>(
                name: "WantKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HaveKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles",
                column: "HaveKidsId",
                principalTable: "HaveKids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles",
                column: "WantKidsId",
                principalTable: "WantKids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
