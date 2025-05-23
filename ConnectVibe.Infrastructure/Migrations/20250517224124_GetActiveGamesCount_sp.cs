using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetActiveGamesCount_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetActiveGamesCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS ActiveGamesCount
    FROM GameSessions
    WHERE Status = 3 -- InProgress
    AND IsDeleted = 0;
END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS sp_GetActiveGamesCount");
        }
    }
}
