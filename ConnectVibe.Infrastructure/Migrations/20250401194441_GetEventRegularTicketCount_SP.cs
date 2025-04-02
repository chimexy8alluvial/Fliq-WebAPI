using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventRegularTicketCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

            CREATE PROCEDURE [dbo].[GetEventRegularTicketCount]
                @EventId INT
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT COUNT(*)
                FROM [dbo].[Tickets]
                WHERE EventId = @EventId
                AND TicketType = 0; -- 0 corresponds to TicketType.Regular
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventRegularTicketCount;");
        }
    }
}
