using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllEventsForDashBoard_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                            CREATE PROCEDURE sp_GetAllEventsForDashBoard
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
                        e.EventTitle,
                        e.UserId,
                        e.StartDate,
                        e.EndDate,
                        e.EventCategory,
                        e.DateCreated,
                        COUNT(t.Id) as TicketCount
                       
                     FROM Events e
                    INNER JOIN Users u ON e.UserId = u.Id
                    LEFT JOIN Tickets t ON e.Id = t.EventId
	                LEFT JOIN LocationDetails ld on e.LocationId = ld.LocationId
                    WHERE (@category IS NULL OR e.EventCategory = @category)
                    AND (@startDate IS NULL OR e.StartDate >= @startDate)
                    AND (@endDate IS NULL OR e.EndDate <= @endDate)
                    AND (@location IS NULL OR ld.Status LIKE '%' + @location + '%')
                    AND e.IsCancelled = 0
                    AND e.IsFlagged = 0
                    AND e.IsDeleted = 0
                    GROUP BY 
                        e.EventTitle,
                        e.UserId,
                        e.StartDate,
                        e.EndDate,
                        e.EventCategory,
                        e.DateCreated
                        
                    ORDER BY e.StartDate DESC
                    OFFSET (@pageNumber - 1) * @pageSize ROWS
                    FETCH NEXT @pageSize ROWS ONLY;
                END

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllEventsForDashBoard");
        }
    }
}
