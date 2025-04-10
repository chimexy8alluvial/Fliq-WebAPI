using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventWeeklyTicketCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[GetEventWeeklyTicketCount]
                @EventId INT,
                @StartDate DATE = NULL,
                @EndDate DATE = NULL,
                @TicketType INT = NULL
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT 
                    DATEPART(WEEKDAY, et.DateCreated) AS DayOfWeek,
                    COUNT(*) AS TicketCount
                FROM [dbo].[EventTickets] et
                INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                WHERE t.EventId = @EventId
                AND (@StartDate IS NULL OR et.DateCreated >= @StartDate)
                AND (@EndDate IS NULL OR et.DateCreated <= @EndDate)
                AND (@TicketType IS NULL OR t.TicketType = @TicketType)
                AND et.IsRefunded = 0
                GROUP BY DATEPART(WEEKDAY, et.DateCreated);
            END
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventWeeklyTicketCount;");
        }
    }
}
