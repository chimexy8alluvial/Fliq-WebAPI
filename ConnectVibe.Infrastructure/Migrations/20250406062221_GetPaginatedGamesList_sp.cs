using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetPaginatedGamesList_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE sp_GetPaginatedGamesList
    @Status INT = NULL,
    @DatePlayedFrom DATETIME = NULL,
    @DatePlayedTo DATETIME = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #FilteredGames
    (
        RowNum INT IDENTITY(1,1),
        GameTitle NVARCHAR(255),
        Players NVARCHAR(255),
        Status INT,
        Stake NVARCHAR(50),
        Winner NVARCHAR(255),
        DatePlayed DATETIME
    );

    INSERT INTO #FilteredGames
    (GameTitle, Players, Status, Stake, Winner, DatePlayed)
    SELECT 
        g.Name AS GameTitle,
        p1.FirstName + ' ' + p1.LastName + ' Vs ' + p2.FirstName + ' ' + p2.LastName AS Players,
        gs.Status AS Status,
        CASE 
            WHEN s.Amount IS NOT NULL THEN '$' + CAST(s.Amount AS NVARCHAR(50)) 
            ELSE NULL 
        END AS Stake,
        CASE 
            WHEN gs.WinnerId IS NOT NULL THEN w.FirstName + ' ' + w.LastName 
            ELSE NULL 
        END AS Winner,
        gs.EndTime AS DatePlayed
    FROM Games g
    INNER JOIN GameSessions gs ON g.Id = gs.GameId
    INNER JOIN Users p1 ON gs.Player1Id = p1.Id
    INNER JOIN Users p2 ON gs.Player2Id = p2.Id
    LEFT JOIN Stakes s ON gs.Id = s.GameSessionId -- Use existing Stakes table
    LEFT JOIN Users w ON gs.WinnerId = w.Id
    WHERE 
        (@Status IS NULL OR gs.Status = @Status)
        AND (@DatePlayedFrom IS NULL OR gs.EndTime >= @DatePlayedFrom)
        AND (@DatePlayedTo IS NULL OR gs.EndTime <= @DatePlayedTo);

    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) FROM #FilteredGames;

    SELECT 
        GameTitle,
        Players,
        Status,
        Stake,
        Winner,
        DatePlayed
    FROM #FilteredGames
    WHERE RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)
    ORDER BY RowNum;

    SELECT @TotalCount AS TotalCount;

    DROP TABLE #FilteredGames;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetPaginatedGamesList");
        }
    }
}
