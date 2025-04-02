using System;
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
                FROM [dbo].[Tickets]
                WHERE EventId = @EventId
                AND TicketType = 3; -- 3 corresponds to TicketType.Other
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
