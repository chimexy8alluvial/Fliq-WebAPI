﻿using Microsoft.EntityFrameworkCore.Migrations;

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
                           CREATE PROCEDURE [dbo].[sp_GetAllFlaggedEventsForDashBoard]
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

                    DECLARE @CurrentDate DATETIME = GETDATE();

                    SELECT DISTINCT
                        e.EventTitle,
                        u.FirstName + ' ' + u.LastName AS CreatedBy,
                        CASE 
                            WHEN e.Status = 4 THEN 'Cancelled'
                            WHEN e.Status = 0 THEN 'PendingApproval'
                            WHEN e.StartDate > @CurrentDate THEN 'Upcoming'
                            WHEN e.StartDate <= @CurrentDate AND e.EndDate >= @CurrentDate THEN 'Ongoing'
                            ELSE 'Past'
                        END AS Status,
                        COUNT(et.Id) AS Attendees, -- Changed to EventTickets
                        CASE 
                            WHEN e.SponsoredEvent = 1 THEN 'sponsored' 
                            ELSE 'free' 
                        END AS Type,
                        e.DateCreated AS CreatedOn
                    FROM Events e
                    INNER JOIN Users u ON e.UserId = u.Id
                    LEFT JOIN Tickets t ON e.Id = t.EventId
                    LEFT JOIN EventTickets et ON et.TicketId = t.Id -- Changed to EventTickets
                    LEFT JOIN [dbo].[LocationDetails] ld ON e.LocationId = ld.LocationId
                    LEFT JOIN [dbo].[LocationResult] lr ON ld.Id = lr.LocationDetailId -- Join to get FormattedAddress
                    WHERE 
                        e.IsDeleted = 0
                        AND e.IsFlagged = 1  -- Only flagged events
                        AND (@category IS NULL OR e.EventCategory = @category)
                        AND (@Status IS NULL OR 
                            CASE 
                                WHEN e.Status = 4 THEN 4
                                WHEN e.Status = 0 THEN 0
                                WHEN e.StartDate > @CurrentDate THEN 1
                                WHEN e.StartDate <= @CurrentDate AND e.EndDate >= @CurrentDate THEN 2
                                ELSE 3
                            END = @Status)
                        AND (@startDate IS NULL OR e.StartDate >= @startDate)
                        AND (@endDate IS NULL OR e.EndDate <= @endDate)
                         AND (@location IS NULL OR lr.FormattedAddress LIKE '%' + @location + '%')
                    GROUP BY 
                        e.EventTitle,
                        u.FirstName,
                        u.LastName,
                        e.StartDate,
                        e.EndDate,
                        e.SponsoredEvent,
                        e.DateCreated,
                        e.Status
                    ORDER BY e.DateCreated DESC
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
