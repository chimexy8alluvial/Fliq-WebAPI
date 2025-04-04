using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventTicketNetRevenue_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             CREATE PROCEDURE [dbo].[GetEventTicketNetRevenue]
                @EventId INT
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT 
                    ISNULL((
                        -- Total amount from non-refunded tickets after discounts
                        SELECT SUM(
                            CASE 
                                WHEN d.Percentage IS NOT NULL 
                                THEN t.Amount * (1 - d.Percentage / 100) -- Apply discount percentage
                                ELSE t.Amount -- No discount applied
                            END
                        )
                        FROM [dbo].[EventTickets] et
                        INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                        LEFT JOIN [dbo].[Discounts] d ON d.Id IN (
                            SELECT value 
                            FROM OPENJSON((
                                SELECT JSON_QUERY(t.Discounts) -- Assuming Discounts is stored as JSON
                            ))
                        )
                        WHERE t.EventId = @EventId
                        AND et.IsRefunded = 0 -- Updated to use EventTicket's IsRefunded
                    ), 0) 
                    - 
                    ISNULL((
                        -- Total amount from refunded tickets (original amount, no discount applied)
                        SELECT SUM(t.Amount)
                        FROM [dbo].[EventTickets] et
                        INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                        WHERE t.EventId = @EventId
                        AND et.IsRefunded = 1 -- Updated to use EventTicket's IsRefunded
                    ), 0) AS NetRevenue;
            END
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventTicketNetRevenue;");
        }
    }
}
