using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetTotalGamersCount_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetTotalGamersCount
AS
BEGIN
    SET NOCOUNT ON;

    -- Get count of distinct players from GameSessions table
    DECLARE @TotalGamersCount INT;

    SELECT @TotalGamersCount = COUNT(DISTINCT PlayerId)
    FROM
    (
        -- Union all player IDs from both Player1Id and Player2Id columns
        SELECT Player1Id AS PlayerId
        FROM GameSessions
        WHERE Player1Id IS NOT NULL
        
        UNION
        
        SELECT Player2Id AS PlayerId
        FROM GameSessions
        WHERE Player2Id IS NOT NULL
    ) AS AllPlayers;

    -- Return the total count
    SELECT @TotalGamersCount AS TotalGamersCount;
END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetTotalGamersCount");
        }
    }
}
