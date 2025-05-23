using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaveCachedRecommendationsSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sPSaveCachedRecommendations
                @UserId INT,
                @RecommendationsJson NVARCHAR(MAX)
            AS
            BEGIN
                SET NOCOUNT ON;
                
                -- Parse JSON and insert recommendations
                INSERT INTO CachedRecommendations (
                    UserId,
                    RecommendationType,
                    EventId,
                    BlindDateId,
                    SpeedDatingEventId,
                    RecommendedUserId,
                    Score,
                    ComputedAt,
                    IsActive,
                    DateCreated,
                    IsDeleted
                )
                SELECT 
                    @UserId,
                    JSON_VALUE(value, '$.RecommendationType'),
                    CASE WHEN JSON_VALUE(value, '$.EventId') = 'null' THEN NULL 
                         ELSE CAST(JSON_VALUE(value, '$.EventId') AS INT) END,
                    CASE WHEN JSON_VALUE(value, '$.BlindDateId') = 'null' THEN NULL 
                         ELSE CAST(JSON_VALUE(value, '$.BlindDateId') AS INT) END,
                    CASE WHEN JSON_VALUE(value, '$.SpeedDatingEventId') = 'null' THEN NULL 
                         ELSE CAST(JSON_VALUE(value, '$.SpeedDatingEventId') AS INT) END,
                    CASE WHEN JSON_VALUE(value, '$.RecommendedUserId') = 'null' THEN NULL 
                         ELSE CAST(JSON_VALUE(value, '$.RecommendedUserId') AS INT) END,
                    CAST(JSON_VALUE(value, '$.Score') AS FLOAT),
                    CAST(JSON_VALUE(value, '$.ComputedAt') AS DATETIME2),
                    CAST(JSON_VALUE(value, '$.IsActive') AS BIT),
                    GETUTCDATE(),
                    0
                FROM OPENJSON(@RecommendationsJson);
            END
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPSaveCachedRecommendations");
        }
    }
}
