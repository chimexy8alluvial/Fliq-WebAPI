using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBlindDateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlindDateCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlindDateCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlindDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    BlindDateCategoryId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    IsOneOnOne = table.Column<bool>(type: "bit", nullable: false),
                    NumberOfParticipants = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRecordingEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RecordingUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SessionEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlindDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlindDates_BlindDateCategories_BlindDateCategoryId",
                        column: x => x.BlindDateCategoryId,
                        principalTable: "BlindDateCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlindDates_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlindDatesParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlindDateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlindDatesParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlindDatesParticipants_BlindDates_BlindDateId",
                        column: x => x.BlindDateId,
                        principalTable: "BlindDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlindDatesParticipants_Users_UserId",
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

            migrationBuilder.CreateIndex(
                name: "IX_BlindDates_BlindDateCategoryId",
                table: "BlindDates",
                column: "BlindDateCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlindDates_LocationId",
                table: "BlindDates",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BlindDatesParticipants_BlindDateId",
                table: "BlindDatesParticipants",
                column: "BlindDateId");

            migrationBuilder.CreateIndex(
                name: "IX_BlindDatesParticipants_UserId",
                table: "BlindDatesParticipants",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlindDatesParticipants");

            migrationBuilder.DropTable(
                name: "BlindDates");

            migrationBuilder.DropTable(
                name: "BlindDateCategories");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 2, 16, 6, 52, 58, 113, DateTimeKind.Utc).AddTicks(1345));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 2, 16, 6, 52, 58, 113, DateTimeKind.Utc).AddTicks(1355));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 2, 16, 6, 52, 58, 113, DateTimeKind.Utc).AddTicks(1357));
        }
    }
}
