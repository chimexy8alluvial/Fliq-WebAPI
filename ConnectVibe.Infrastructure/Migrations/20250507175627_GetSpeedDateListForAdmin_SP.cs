using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetSpeedDateListForAdmin_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                        CREATE OR ALTER PROCEDURE sp_SpeedDateListForAdmin
                @PageSize INT,
                @PageNumber INT,
                @CreationStatus INT = NULL
            AS
            BEGIN
                -- Calculate the number of rows to skip
                DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
                
                -- Select speed dating events with pagination and optional filtering by creation status
                SELECT 
                    sde.Id,
                    sde.Title,
                    sde.Category,
                    sde.StartTime,
                    sde.StartSessionTime,
                    sde.EndSessionTime,
                    sde.ImageUrl,
                    sde.MinAge,
                    sde.MaxAge,
                    sde.MaxParticipants,
                    sde.DurationPerPairingMinutes,
                    sde.Status,
                    sde.IsFlagged,
                    sde.CreatorIsAdmin,
                    sde.CreatedByUserId,
                    sde.ContentCreationStatus,
                    sde.ApprovedAt,
                    sde.ApprovedByUserId,
                    sde.RejectionReason,
                    sde.LocationId,
                    sde.DateCreated,
                    sde.DateModified,
                    sde.IsDeleted,
                    
                    -- Location information
                    l.Id AS LocationId,   
                    l.Lat AS LocationLatitude,
                    l.Lng AS LocationLongitude

                FROM 
                    SpeedDatingEvents sde
                LEFT JOIN
                    Location l ON sde.LocationId = l.Id
                WHERE 
                    sde.IsDeleted = 0
                    AND (@CreationStatus IS NULL OR sde.ContentCreationStatus = @CreationStatus)
                ORDER BY 
                    sde.DateCreated DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
                
                -- Get the total count for pagination metadata (optional)
                SELECT COUNT(*) AS TotalCount
                FROM SpeedDatingEvents sde
                WHERE 
                    sde.IsDeleted = 0
                    AND (@CreationStatus IS NULL OR sde.ContentCreationStatus = @CreationStatus);
            END
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_SpeedDateListForAdmin;");
        }
    }
}
