using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventMondayTicketCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[GetEventMondayTicketCount]
                @EventId INT,
                @TicketType INT = NULL
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT COUNT(*)
                FROM [dbo].[Tickets]
                WHERE EventId = @EventId
                AND DATEPART(WEEKDAY, DateSold) = 2 -- Monday (Sunday = 1 in SQL Server default)
                AND (@TicketType IS NULL OR TicketType = @TicketType);
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventMondayTicketCount;");
        }
    }
}
