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

                -- Default to last 7 days if dates are null
                DECLARE @EffectiveEndDate DATE = COALESCE(@EndDate, CAST(GETUTCDATE() AS DATE));
                DECLARE @EffectiveStartDate DATE = COALESCE(@StartDate, DATEADD(DAY, -6, @EffectiveEndDate));

                -- Define all 7 days (0=Sunday, 6=Saturday)
                WITH AllDays AS (
                    SELECT DayOfWeek
                    FROM (VALUES (0), (1), (2), (3), (4), (5), (6)) AS Days(DayOfWeek)
                )
                SELECT 
                    d.DayOfWeek,
                    ISNULL(COUNT(t.Id), 0) AS TicketCount
                FROM AllDays d
                LEFT JOIN [dbo].[EventTickets] et 
                    ON (DATEPART(WEEKDAY, et.DateCreated) + 5) % 7 = d.DayOfWeek
                    AND et.DateCreated >= @EffectiveStartDate
                    AND et.DateCreated <= @EffectiveEndDate
                    AND et.IsRefunded = 0
                LEFT JOIN [dbo].[Tickets] t 
                    ON et.TicketId = t.Id 
                 INNER JOIN [dbo].[Events] e ON e.Id = t.EventId
                    AND t.EventId = @EventId
                     AND e.IsDeleted = 0
                    AND (t.TicketType = @TicketType OR @TicketType IS NULL)
                GROUP BY d.DayOfWeek
                ORDER BY d.DayOfWeek;
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
