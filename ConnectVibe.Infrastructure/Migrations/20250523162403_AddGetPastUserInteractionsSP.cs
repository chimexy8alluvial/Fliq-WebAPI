using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetPastUserInteractionsSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sPGetPastUserInteractions
                @UserId INT,
                @EventType NVARCHAR(50)
            AS
            BEGIN
                SET NOCOUNT ON;
                
                -- Get user interactions based on event type
                IF @EventType = 'event'
                BEGIN
                    SELECT 
                        ui.Id,
                        ui.UserId,
                        ui.Type,
                        ui.EventId,
                        ui.BlindDateId,
                        ui.SpeedDatingEventId,
                        ui.InteractedWithUserId,
                        ui.InteractionTime,
                        ui.InteractionStrength,
                        ui.DateCreated,
                        ui.DateModified,
                        ui.IsDeleted
                    FROM UserInteractions ui
                    WHERE ui.UserId = @UserId 
                        AND ui.EventId IS NOT NULL
                        AND ui.IsDeleted = 0
                    ORDER BY ui.InteractionTime DESC;
                END
                ELSE IF @EventType = 'blinddate'
                BEGIN
                    SELECT 
                        ui.Id,
                        ui.UserId,
                        ui.Type,
                        ui.EventId,
                        ui.BlindDateId,
                        ui.SpeedDatingEventId,
                        ui.InteractedWithUserId,
                        ui.InteractionTime,
                        ui.InteractionStrength,
                        ui.DateCreated,
                        ui.DateModified,
                        ui.IsDeleted
                    FROM UserInteractions ui
                    WHERE ui.UserId = @UserId 
                        AND ui.BlindDateId IS NOT NULL
                        AND ui.IsDeleted = 0
                    ORDER BY ui.InteractionTime DESC;
                END
                ELSE IF @EventType = 'speeddate'
                BEGIN
                    SELECT 
                        ui.Id,
                        ui.UserId,
                        ui.Type,
                        ui.EventId,
                        ui.BlindDateId,
                        ui.SpeedDatingEventId,
                        ui.InteractedWithUserId,
                        ui.InteractionTime,
                        ui.InteractionStrength,
                        ui.DateCreated,
                        ui.DateModified,
                        ui.IsDeleted
                    FROM UserInteractions ui
                    WHERE ui.UserId = @UserId 
                        AND ui.SpeedDatingEventId IS NOT NULL
                        AND ui.IsDeleted = 0
                    ORDER BY ui.InteractionTime DESC;
                END
                ELSE IF @EventType = 'user'
                BEGIN
                    SELECT 
                        ui.Id,
                        ui.UserId,
                        ui.Type,
                        ui.EventId,
                        ui.BlindDateId,
                        ui.SpeedDatingEventId,
                        ui.InteractedWithUserId,
                        ui.InteractionTime,
                        ui.InteractionStrength,
                        ui.DateCreated,
                        ui.DateModified,
                        ui.IsDeleted
                    FROM UserInteractions ui
                    WHERE ui.UserId = @UserId 
                        AND ui.InteractedWithUserId IS NOT NULL
                        AND ui.IsDeleted = 0
                    ORDER BY ui.InteractionTime DESC;
                END
            END
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPGetPastUserInteractions");
        }
    }
}
