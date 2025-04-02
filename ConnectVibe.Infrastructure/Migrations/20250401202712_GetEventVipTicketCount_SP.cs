using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventVipTicketCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[GetEventVipTicketCount]
                @EventId INT
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT COUNT(*)
                FROM [dbo].[Tickets]
                WHERE EventId = @EventId
                AND TicketType = 1; -- 1 corresponds to TicketType.Vip
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventVipTicketCount;");
        }
    }
}
