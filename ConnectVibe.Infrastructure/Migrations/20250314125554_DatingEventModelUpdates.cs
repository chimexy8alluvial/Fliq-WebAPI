using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DatingEventModelUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpeedDatingEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartSessionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndSessionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    MaxAge = table.Column<int>(type: "int", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    DurationPerPairingMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeedDatingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeedDatingEvents_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpeedDatingParticipanticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HasCompletedAllRounds = table.Column<bool>(type: "bit", nullable: false),
                    IsCreator = table.Column<bool>(type: "bit", nullable: false),
                    SpeedDatingEventId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeedDatingParticipanticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeedDatingParticipanticipants_SpeedDatingEvents_SpeedDatingEventId",
                        column: x => x.SpeedDatingEventId,
                        principalTable: "SpeedDatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeedDatingParticipanticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 3, 14, 12, 55, 49, 912, DateTimeKind.Utc).AddTicks(8001));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 3, 14, 12, 55, 49, 912, DateTimeKind.Utc).AddTicks(8012));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 3, 14, 12, 55, 49, 912, DateTimeKind.Utc).AddTicks(8014));

            migrationBuilder.CreateIndex(
                name: "IX_SpeedDatingEvents_LocationId",
                table: "SpeedDatingEvents",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeedDatingParticipanticipants_SpeedDatingEventId",
                table: "SpeedDatingParticipanticipants",
                column: "SpeedDatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeedDatingParticipanticipants_UserId",
                table: "SpeedDatingParticipanticipants",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeedDatingParticipanticipants");

            migrationBuilder.DropTable(
                name: "SpeedDatingEvents");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 55, 48, 839, DateTimeKind.Utc).AddTicks(9278));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 55, 48, 839, DateTimeKind.Utc).AddTicks(9280));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 2, 20, 22, 55, 48, 839, DateTimeKind.Utc).AddTicks(9281));
        }
    }
}
