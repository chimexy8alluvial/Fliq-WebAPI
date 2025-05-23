using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetCachedRecommendationsSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sPGetCachedRecommendations
                @UserId INT,
                @RecommendationType NVARCHAR(50),
                @Count INT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                SELECT TOP (@Count)
                    cr.Id,
                    cr.UserId,
                    cr.RecommendationType,
                    cr.EventId,
                    cr.BlindDateId,
                    cr.SpeedDatingEventId,
                    cr.RecommendedUserId,
                    cr.Score,
                    cr.ComputedAt,
                    cr.IsActive,
                    cr.DateCreated,
                    cr.DateModified,
                    cr.IsDeleted
                FROM CachedRecommendations cr
                WHERE cr.UserId = @UserId
                    AND cr.RecommendationType = @RecommendationType
                    AND cr.IsActive = 1
                    AND cr.IsDeleted = 0
                ORDER BY cr.Score DESC, cr.ComputedAt DESC;
            END
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPGetCachedRecommendations");
        }
    }
}
