using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClearCachedRecommendationsSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sPClearCachedRecommendations
                @UserId INT,
                @DeletedCount INT OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                
                -- Soft delete approach (set IsDeleted = 1)
                UPDATE CachedRecommendations 
                SET IsDeleted = 1,
                    DateModified = GETUTCDATE()
                WHERE UserId = @UserId 
                    AND IsDeleted = 0;
                
                SET @DeletedCount = @@ROWCOUNT;
                
            END
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPClearCachedRecommendations");
        }
    }
}
