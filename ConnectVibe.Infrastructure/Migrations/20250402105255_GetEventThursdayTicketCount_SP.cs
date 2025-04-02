using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventThursdayTicketCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             CREATE PROCEDURE [dbo].[GetEventThursdayTicketCount]
                @EventId INT,
                @TicketType INT = NULL
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT COUNT(*)
                FROM [dbo].[Tickets]
                WHERE EventId = @EventId
                AND DATEPART(WEEKDAY, DateCreated) = 5 -- Thursday
                AND IsRefunded = 0
                AND (@TicketType IS NULL OR TicketType = @TicketType);
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventThursdayTicketCount;");
        }
    }
}
