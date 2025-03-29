using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllCancelledEventsForDashBoard_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                            CREATE PROCEDURE sp_GetAllCancelledEventsForDashBoard
                    @pageNumber INT,
                    @pageSize INT,
                    @category VARCHAR(50) = NULL,
                    @startDate DATETIME = NULL,
                    @endDate DATETIME = NULL,
                    @location VARCHAR(255) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                     SELECT 
					e.Id,
					e.EventTitle,
					e.UserId,
					e.StartDate,
					e.EndDate,
					e.SponsoredEvent,
					e.DateCreated,
					u.FirstName,
					u.LastName,
					COUNT(t.Id) AS TicketCount
				FROM Events e
				INNER JOIN Users u ON e.UserId = u.Id
				LEFT JOIN Tickets t ON e.Id = t.EventId
				LEFT JOIN LocationDetails ld ON e.LocationId = ld.LocationId
				WHERE 
					(@category IS NULL OR e.EventCategory = @category) 
					AND (@startDate IS NULL OR e.StartDate >= @startDate)
					AND (@endDate IS NULL OR e.EndDate <= @endDate)
					AND (@location IS NULL OR ld.Status LIKE '%' + @location + '%')
					AND e.IsCancelled = 1
			
				GROUP BY 
					e.Id,
					e.EventTitle,
					e.UserId,
					e.StartDate,
					e.EndDate,
					e.SponsoredEvent,
					e.DateCreated,
					u.FirstName,
					u.LastName
                        
                    ORDER BY e.StartDate DESC
                    OFFSET (@pageNumber - 1) * @pageSize ROWS
                    FETCH NEXT @pageSize ROWS ONLY;
                END

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllCancelledEventsForDashBoard");
        }
    }
}
