using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventFridayTicketCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             CREATE PROCEDURE [dbo].[GetEventFridayTicketCount]
                @EventId INT,
                @TicketType INT = NULL
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT COUNT(*)
                FROM [dbo].[Tickets]
                WHERE EventId = @EventId
                AND DATEPART(WEEKDAY, DateSold) = 6 -- Friday
                AND (@TicketType IS NULL OR TicketType = @TicketType)
                AND IsRefunded = 0;
            END
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventFridayTicketCount;");
        }
    }
}
