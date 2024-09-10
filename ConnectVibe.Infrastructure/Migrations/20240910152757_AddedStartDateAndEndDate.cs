using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedStartDateAndEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventsId",
                table: "ProfilePhoto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndAge",
                table: "events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartAge",
                table: "events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePhoto_EventsId",
                table: "ProfilePhoto",
                column: "EventsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfilePhoto_events_EventsId",
                table: "ProfilePhoto",
                column: "EventsId",
                principalTable: "events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfilePhoto_events_EventsId",
                table: "ProfilePhoto");

            migrationBuilder.DropIndex(
                name: "IX_ProfilePhoto_EventsId",
                table: "ProfilePhoto");

            migrationBuilder.DropColumn(
                name: "EventsId",
                table: "ProfilePhoto");

            migrationBuilder.DropColumn(
                name: "EndAge",
                table: "events");

            migrationBuilder.DropColumn(
                name: "StartAge",
                table: "events");
        }
    }
}
