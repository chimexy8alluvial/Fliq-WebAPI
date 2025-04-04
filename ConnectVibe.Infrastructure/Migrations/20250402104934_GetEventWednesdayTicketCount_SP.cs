﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventWednesdayTicketCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[GetEventWednesdayTicketCount]
                @EventId INT,
                @TicketType INT = NULL
            AS
            BEGIN
                SET NOCOUNT ON;

                 SELECT COUNT(*)
                FROM [dbo].[EventTickets] et
                INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                WHERE t.EventId = @EventId
                AND DATEPART(WEEKDAY, et.DateCreated) = 4 -- Wednesday (using EventTicket's DateCreated)
                AND (@TicketType IS NULL OR t.TicketType = @TicketType)
                AND et.IsRefunded = 0; -- Updated to use EventTicket's IsRefunded
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventWednesdayTicketCount;");
        }
    }
}
