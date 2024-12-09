using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetGameHistorySP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
      CREATE PROCEDURE GetGameHistoryByTwoPlayers
    @Player1Id INT, -- The ID of the first player
    @Player2Id INT  -- The ID of the second player
AS
BEGIN
    -- Retrieve game history between Player1Id and Player2Id
    SELECT
        gh.Id AS HistoryId,
        g.Name AS GameName,
        gh.StartTime,
        gh.EndTime,
        gh.Player1Score,
        gh.Player2Score
    FROM
        GameSessions gh
    INNER JOIN
        Games g ON gh.GameId = g.Id
    WHERE
        -- Matches either Player1 vs Player2 or Player2 vs Player1
        (gh.Player1Id = @Player1Id AND gh.Player2Id = @Player2Id)
        OR (gh.Player1Id = @Player2Id AND gh.Player2Id = @Player1Id)
    ORDER BY
        gh.StartTime DESC; -- Sort by most recent history first
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}