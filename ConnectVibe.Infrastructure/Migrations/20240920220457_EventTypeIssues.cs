using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EventTypeIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Event_Type",
                table: "EventCriterias",
                newName: "EventType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "EventCriterias",
                newName: "Event_Type");
        }
    }
}
