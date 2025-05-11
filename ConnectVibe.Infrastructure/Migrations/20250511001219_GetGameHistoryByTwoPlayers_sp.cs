using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetGameHistoryByTwoPlayers_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetGameHistoryByTwoPlayers
    @Player1Id INT,
    @Player2Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        gs.Id AS HistoryId,
        g.Name AS GameName,
        gs.GameId,
        gs.StartTime,
        gs.EndTime,
        gs.Player1Score,
        gs.Player2Score,
        gs.Player1Id,
        gs.Player2Id,
        gs.Status
    FROM 
        GameSessions gs
        INNER JOIN Games g ON gs.GameId = g.Id
    WHERE 
        (
            (gs.Player1Id = @Player1Id AND gs.Player2Id = @Player2Id)
            OR
            (gs.Player1Id = @Player2Id AND gs.Player2Id = @Player1Id)
        )
        AND gs.IsDeleted = 0
        AND g.IsDeleted = 0
    ORDER BY 
        gs.DateCreated DESC;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetGameHistoryByTwoPlayers");
        }
    }
}
