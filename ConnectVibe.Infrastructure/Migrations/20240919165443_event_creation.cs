using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class event_creation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDocumentVerified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventCriterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Event_Type = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Race = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCriterias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SponsoredEventDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfromation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SponsoringBudget = table.Column<int>(type: "int", nullable: false),
                    TargetAudienceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfInvitees = table.Column<int>(type: "int", nullable: false),
                    Budget = table.Column<double>(type: "float", nullable: false),
                    DurationOfSponsorship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferedLevelOfInvolvement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsoredEventDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    EventTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    StartAge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndAge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SponsoredEvent = table.Column<bool>(type: "bit", nullable: false),
                    SponsoredEventDetailId = table.Column<int>(type: "int", nullable: false),
                    EventCriteriaId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_EventCriterias_EventCriteriaId",
                        column: x => x.EventCriteriaId,
                        principalTable: "EventCriterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_SponsoredEventDetails_SponsoredEventDetailId",
                        column: x => x.SponsoredEventDetailId,
                        principalTable: "SponsoredEventDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventMedias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMedias_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpensOn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosesOn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    TicketTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    EventsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTypes_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketTypes_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventMedias_EventsId",
                table: "EventMedias",
                column: "EventsId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventCriteriaId",
                table: "Events",
                column: "EventCriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LocationId",
                table: "Events",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SponsoredEventDetailId",
                table: "Events",
                column: "SponsoredEventDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_EventsId",
                table: "TicketTypes",
                column: "EventsId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_LocationId",
                table: "TicketTypes",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventMedias");

            migrationBuilder.DropTable(
                name: "TicketTypes");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "EventCriterias");

            migrationBuilder.DropTable(
                name: "SponsoredEventDetails");

            migrationBuilder.DropColumn(
                name: "IsDocumentVerified",
                table: "Users");
        }
    }
}
