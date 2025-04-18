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
            CREATE OR ALTER PROCEDURE [dbo].[GetEventTicketNetRevenue]
                    @EventId INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        ISNULL(SUM(
                            CASE 
                                WHEN et.IsRefunded = 0 THEN 
                                    CASE 
                                        WHEN d.Percentage IS NOT NULL THEN t.Amount * (1 - d.Percentage / 100.0)
                                        ELSE t.Amount
                                    END
                                ELSE -t.Amount
                            END
                        ), 0) AS NetRevenue
                    FROM [dbo].[EventTickets] et
                    INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                    LEFT JOIN [dbo].[Discount] d ON d.TicketId = t.Id AND d.IsDeleted = 0
                    WHERE t.EventId = @EventId;
                END;
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventTicketNetRevenue;");
        }
    }
}
