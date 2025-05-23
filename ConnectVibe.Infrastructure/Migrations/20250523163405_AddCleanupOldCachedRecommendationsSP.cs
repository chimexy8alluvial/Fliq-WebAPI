using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCleanupOldCachedRecommendationsSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sPCleanupOldCachedRecommendations
                @OlderThan DATETIME2,
                @DeletedCount INT OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                -- Hard delete old recommendations (they're no longer useful)
                DELETE FROM CachedRecommendations 
                WHERE ComputedAt < @OlderThan;
                
                SET @DeletedCount = @@ROWCOUNT;
            END
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPCleanupOldCachedRecommendations");
        }
    }
}
