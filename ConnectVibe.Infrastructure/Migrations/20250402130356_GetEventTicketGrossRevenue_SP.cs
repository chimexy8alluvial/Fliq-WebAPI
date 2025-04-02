using System;
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

                    SELECT ISNULL(SUM(Amount), 0)
                    FROM [dbo].[Tickets]
                    WHERE EventId = @EventId
                    AND IsRefunded = 0; -- Exclude refunded tickets
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
