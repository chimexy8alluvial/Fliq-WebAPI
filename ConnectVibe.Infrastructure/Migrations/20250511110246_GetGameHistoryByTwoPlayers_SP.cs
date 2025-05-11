using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetGameHistoryByTwoPlayers_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
           CREATE PROCEDURE GetGameHistoryByTwoPlayers
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
                    gs.Player2Id
                FROM 
                    GameSessions gs
                    INNER JOIN Games g ON gs.GameId = g.Id
                WHERE 
                    (gs.Player1Id = @Player1Id AND gs.Player2Id = @Player2Id)
                    OR (gs.Player1Id = @Player2Id AND gs.Player2Id = @Player1Id)
                    AND gs.Status = 4 -- GameStatus.Done
                ORDER BY 
                    gs.StartTime DESC;
            END;
             ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetGameHistoryByTwoPlayers;");
        }
    }
}
