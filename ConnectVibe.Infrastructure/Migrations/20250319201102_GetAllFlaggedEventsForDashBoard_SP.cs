using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllFlaggedEventsForDashBoard_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                            CREATE PROCEDURE sp_GetAllFlaggedEventsForDashBoard
                    @pageNumber INT,
                    @pageSize INT,
                    @category VARCHAR(50) = NULL,
                    @Status INT = NULL,
                    @startDate DATETIME = NULL,
                    @endDate DATETIME = NULL,
                    @location VARCHAR(255) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                            DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
                            DECLARE @CurrentDate DATETIME = GETDATE();

                    SELECT 
					e.Id,
					e.EventTitle,
					e.UserId,
					e.StartDate,
					e.EndDate,
					e.SponsoredEvent,
					e.DateCreated,
                     CASE 
                        WHEN e.Status = 4 THEN 'Cancelled' -- Cancelled
                        WHEN e.Status = 0 THEN 'PendingApproval' -- PendingApproval
                        WHEN e.StartDate > @CurrentDate THEN 'Upcoming'
                        WHEN e.StartDate <= @CurrentDate AND e.EndDate >= @CurrentDate THEN 'Ongoing'
                        ELSE 'Past'
                    END AS CalculatedStatus,
					u.FirstName,
					u.LastName,
					COUNT(t.Id) AS TicketCount
				FROM Events e
				INNER JOIN Users u ON e.UserId = u.Id
				LEFT JOIN Tickets t ON e.Id = t.EventId
				LEFT JOIN LocationDetails ld ON e.LocationId = ld.LocationId
				WHERE 
					(@category IS NULL OR e.EventCategory = @category)
                    AND (@Status IS NULL OR e.Status = @Status)
					AND (@startDate IS NULL OR e.StartDate >= @startDate)
					AND (@endDate IS NULL OR e.EndDate <= @endDate)
					AND (@location IS NULL OR ld.Status LIKE '%' + @location + '%')
					AND e.IsFlagged = 1
                    AND e.IsDeleted = 0

				GROUP BY 
					e.Id,
					e.EventTitle,
					e.UserId,
					e.StartDate,
					e.EndDate,
					e.SponsoredEvent,
					e.DateCreated,
                    e.Status,
					u.FirstName,
					u.LastName

                    ORDER BY  e.DateCreated DESC
                    OFFSET (@pageNumber - 1) * @pageSize ROWS
                    FETCH NEXT @pageSize ROWS ONLY;
                END

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllFlaggedEventsForDashBoard");
        }
    }
}
