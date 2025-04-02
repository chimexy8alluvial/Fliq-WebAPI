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
                @Category NVARCHAR(100) = NULL,
                @StatusFilter NVARCHAR(50) = NULL, -- 'SoldOut' or 'Cancelled'
                @StartDate DATETIME = NULL,
                @EndDate DATETIME = NULL,
                @Location NVARCHAR(100) = NULL
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH EventTicketsCTE AS (
                    SELECT 
                        e.EventTitle,
                        CreatedBy = u.FirstName + ' ' + u.LastName,
                        EventStatus = CASE e.EventStatus
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
                            FROM [dbo].[Tickets] t 
                            WHERE t.EventId = e.Id 
                            AND t.IsRefund = 0 -- Exclude refunded tickets
                        ), 0),
                        e.DateCreated AS CreatedOn,
                        ROW_NUMBER() OVER (ORDER BY e.DateCreated DESC) AS RowNum
                    FROM 
                        [dbo].[Events] e
                        INNER JOIN [dbo].[Users] u ON e.UserId = u.Id
                        LEFT JOIN LocationDetails ld ON e.LocationId = ld.LocationId
                    WHERE 
                        (@Category IS NULL OR e.EventCategory = @Category)
                        AND (@StartDate IS NULL OR e.StartDate >= @StartDate)
                        AND (@EndDate IS NULL OR e.EndDate <= @EndDate)
                        AND (@Location IS NULL OR ld.Status LIKE '%' + @Location + '%')
                        AND (
                            @StatusFilter IS NULL 
                            OR (@StatusFilter = 'SoldOut' AND EXISTS (
                                SELECT 1 
                                FROM [dbo].[Tickets] t 
                                WHERE t.EventId = e.Id 
                                AND t.SoldOut = 1 
                                AND t.IsRefund = 0 -- Exclude refunded tickets in filter too
                            ))
                            OR (@StatusFilter = 'Cancelled' AND e.EventStatus = 4)
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
