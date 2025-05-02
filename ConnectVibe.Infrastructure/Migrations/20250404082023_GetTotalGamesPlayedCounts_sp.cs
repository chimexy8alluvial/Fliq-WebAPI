using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetTotalGamesPlayedCount_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetTotalGamesPlayedCounts
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS GamesCount
    FROM Games
    WHERE IsDeleted = 0
END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetTotalGamesPlayedCounts");
        }
    }
}
