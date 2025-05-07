using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllEventsTicketsForDashBoard_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
          CREATE PROCEDURE [dbo].[GetAllEventsTicketsForDashBoard]
                    @PageNumber INT,
                    @PageSize INT,
                    @Category INT = NULL, -- Changed to INT to match EventCategory enum
                    @StatusFilter NVARCHAR(50) = NULL, -- 'SoldOut' or 'Cancelled'
                    @StartDate DATETIME = NULL,
                    @EndDate DATETIME = NULL,
                    @Location NVARCHAR(100) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    WITH EventTicketsCTE AS (
                        SELECT DISTINCT -- Avoid duplicates from multiple LocationResults
                            e.EventTitle,
                            CreatedBy = u.FirstName + ' ' + u.LastName,
                            EventStatus = CASE e.Status
                                WHEN 0 THEN 'PendingApproval'
                                WHEN 1 THEN 'Upcoming'
                                WHEN 2 THEN 'Ongoing'
                                WHEN 3 THEN 'Past'
                                WHEN 4 THEN 'Cancelled'
                                ELSE 'Unknown'
                            END,
                            NatureOfEvent = CASE 
                                WHEN e.SponsoredEvent = 1 THEN 'sponsored'
                                ELSE 'free'
                            END,
                            NumOfSoldTickets = ISNULL((
                                SELECT COUNT(*) 
                                FROM [dbo].[EventTickets] et
                                INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                                WHERE t.EventId = e.Id 
                                AND et.IsRefunded = 0 -- Exclude refunded tickets
                            ), 0),
                            e.DateCreated AS CreatedOn,
                            ROW_NUMBER() OVER (ORDER BY e.DateCreated DESC) AS RowNum
                        FROM 
                            [dbo].[Events] e
                            INNER JOIN [dbo].[Users] u ON e.UserId = u.Id
                            LEFT JOIN [dbo].[LocationDetails] ld ON e.LocationId = ld.LocationId
                            LEFT JOIN [dbo].[LocationResult] lr ON ld.Id = lr.LocationDetailId -- Join to get FormattedAddress
                        WHERE 
                            (@Category IS NULL OR e.EventCategory = @Category) -- Compare with enum value
                            AND e.IsDeleted = 0
                            AND (@StartDate IS NULL OR e.StartDate >= @StartDate)
                            AND (@EndDate IS NULL OR e.EndDate <= @EndDate)
                            AND (@Location IS NULL OR lr.FormattedAddress LIKE '%' + @Location + '%') -- Updated filter
                            AND (
                                @StatusFilter IS NULL 
                                OR (@StatusFilter = 'SoldOut' AND (
                                    -- Condition 1: Number of sold tickets meets or exceeds capacity
                                    (SELECT COUNT(*) 
                                     FROM [dbo].[EventTickets] et
                                     INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                                     WHERE t.EventId = e.Id 
                                     AND et.IsRefunded = 0) >= e.Capacity
                                    -- Condition 2: All ticket types for the event are marked SoldOut
                                    OR NOT EXISTS (
                                        SELECT 1 
                                        FROM [dbo].[Tickets] t
                                        WHERE t.EventId = e.Id 
                                        AND t.SoldOut = 0 -- At least one ticket type is not sold out
                                    )
                                ))
                                OR (@StatusFilter = 'Cancelled' AND e.Status = 4)
                            )
                    )
                    SELECT 
                        EventTitle,
                        CreatedBy,
                        EventStatus,
                        NatureOfEvent,
                        NumOfSoldTickets,
                        CreatedOn
                    FROM 
                        EventTicketsCTE
                    WHERE 
                        RowNum BETWEEN ((@PageNumber - 1) * @PageSize + 1) AND (@PageNumber * @PageSize)
                    ORDER BY 
                        CreatedOn DESC;
                END
             ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllEventsTicketsForDashBoard");
        }
    }
}
