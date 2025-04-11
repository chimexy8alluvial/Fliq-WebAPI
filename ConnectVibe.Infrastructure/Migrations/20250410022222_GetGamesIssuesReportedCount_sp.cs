using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetGamesIssuesReportedCountsp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetGamesIssuesReportedCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS GamesIssuesReportedCount
    FROM SupportTickets
    WHERE RequestType = 1
    AND IsDeleted = 0 
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MIGRATION IF EXISTS sp_GetGamesIssuesReportedCount");
        }
    }
}
