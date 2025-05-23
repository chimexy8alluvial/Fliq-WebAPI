using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHasValidCachedRecommendationsSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sPHasValidCachedRecommendations
                @UserId INT,
                @RecommendationType NVARCHAR(50),
                @OlderThan DATETIME2,
                @HasValid BIT OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                DECLARE @Count INT;
                
                SELECT @Count = COUNT(*)
                FROM CachedRecommendations cr
                WHERE cr.UserId = @UserId
                    AND cr.RecommendationType = @RecommendationType
                    AND cr.IsActive = 1
                    AND cr.IsDeleted = 0
                    AND cr.ComputedAt > @OlderThan;
                
                SET @HasValid = CASE WHEN @Count > 0 THEN 1 ELSE 0 END;
            END
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPHasValidCachedRecommendations");
        }
    }
}
