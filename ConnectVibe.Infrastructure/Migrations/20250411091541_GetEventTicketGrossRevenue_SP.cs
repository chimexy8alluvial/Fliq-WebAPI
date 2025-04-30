using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventTicketGrossRevenue_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             CREATE PROCEDURE [dbo].[GetEventTicketGrossRevenue]
                @EventId INT
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT 
                    ISNULL((
                        SELECT SUM(t.Amount)
                        FROM [dbo].[EventTickets] et
                        INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                        INNER JOIN [dbo].[Events] e ON e.Id = t.EventId
                        WHERE t.EventId = @EventId
                        AND e.IsDeleted = 0
                        -- No IsRefunded filter; includes all tickets sold
                    ), 0) AS GrossRevenue;
            END
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventTicketGrossRevenue;");
        }
    }
}
