using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventOtherTicketCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
           CREATE PROCEDURE [dbo].[GetEventOtherTicketCount]
                @EventId INT
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT COUNT(*)
                FROM [dbo].[EventTickets] et
                INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                INNER JOIN [dbo].[Events] e ON e.Id = t.EventId
                WHERE t.EventId = @EventId
                AND e.IsDeleted = 0
               AND et.IsRefunded = 0 -- Updated to use EventTicket's IsRefunded
                AND t.TicketType = 3; -- 3 corresponds to TicketType.Other
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventOtherTicketCount;");
        }
    }
}
