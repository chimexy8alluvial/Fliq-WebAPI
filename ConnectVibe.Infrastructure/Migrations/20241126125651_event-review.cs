using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class eventreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            // Remove the Race column if it exists
            migrationBuilder.DropColumn(
                name: "Race",
                table: "EventCriterias");

            // Add the missing columns
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "EventCriterias",
                nullable: false,
                defaultValueSql: "GETDATE()"); // Sets default value to current date/time

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "EventCriterias",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EventCriterias",
                nullable: false,
                defaultValue: false); // Sets default value to false

            migrationBuilder.CreateTable(
                name: "EventPaymentDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPaymentDetail", x => x.Id);
                });

            // Add the missing SponsoredEvent columns
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "SponsoredEventDetails",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()"); // Sets default value to current date/time

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "SponsoredEventDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SponsoredEventDetails",
                nullable: false,
                defaultValue: false); // Sets default value to false

            // Add new Event columns
            migrationBuilder.AddColumn<string>(
                name: "OccupiedSeats",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MinAge",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxAge",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EventPaymentDetailId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InviteesException",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Events",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Events",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Add foreign key for EventPaymentDetail in Event table
            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventPaymentDetail_EventPaymentDetailId",
                table: "Events",
                column: "EventPaymentDetailId",
                principalTable: "EventPaymentDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Drop old Event table columns
            migrationBuilder.DropColumn(
                name: "StartAge",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EndAge",
                table: "Events");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "EventMedias",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "EventMedias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EventMedias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventReviews_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketType = table.Column<int>(type: "int", nullable: false),
                    TicketDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumLimit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoldOut = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: false),
                    NumberOfTickets = table.Column<int>(type: "int", nullable: true),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discount_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTickets_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTickets_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTickets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discount_TicketId",
                table: "Discount",
                column: "TicketId");


            migrationBuilder.CreateIndex(
                name: "IX_EventReviews_EventId",
                table: "EventReviews",
                column: "EventId");


            migrationBuilder.CreateIndex(
                name: "IX_Events_EventPaymentDetailId",
                table: "Events",
                column: "EventPaymentDetailId");


            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_PaymentId",
                table: "EventTickets",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_TicketId",
                table: "EventTickets",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_UserId",
                table: "EventTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CurrencyId",
                table: "Tickets",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EventId",
                table: "Tickets",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "EventMedias");

            migrationBuilder.DropTable(
                name: "EventReviews");

            migrationBuilder.DropTable(
                name: "EventTickets");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Events");

            // Recreate the Race column
            migrationBuilder.AddColumn<int>(
                name: "Race",
                table: "EventCriterias",
                nullable: true);

            // Remove the newly added columns
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "EventCriterias");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "EventCriterias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EventCriterias");

            // Remove the newly added columns
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "SponsoredEventDetails");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "SponsoredEventDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SponsoredEventDetails");

            migrationBuilder.DropTable(
                name: "EventPaymentDetail");

            migrationBuilder.DropTable(
                name: "SponsoredEventDetails");

            // Remove new columns
            migrationBuilder.DropColumn(name: "OccupiedSeats", table: "Events");
            migrationBuilder.DropColumn(name: "MinAge", table: "Events");
            migrationBuilder.DropColumn(name: "MaxAge", table: "Events");
            migrationBuilder.DropColumn(name: "EventPaymentDetailId", table: "Events");
            migrationBuilder.DropColumn(name: "InviteesException", table: "Events");
            migrationBuilder.DropColumn(name: "DateCreated", table: "Events");
            migrationBuilder.DropColumn(name: "DateModified", table: "Events");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Events");

            // Remove foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventPaymentDetail_EventPaymentDetailId",
                table: "Events");


            migrationBuilder.DropColumn(name: "DateCreated", table: "EventMedias");
            migrationBuilder.DropColumn(name: "DateModified", table: "EventMedias");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "EventMedias");
        }
    }
}
