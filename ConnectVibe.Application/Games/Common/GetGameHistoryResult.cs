namespace Fliq.Application.Games.Common
{
    public record GetGameHistoryResult(
     int HistoryId,
     string GameName,
     int GameId,
     DateTime StartTime,
     DateTime? EndTime,
     int Player1Score,
     int Player2Score,
     int Player1Id,
     int Player2Id
 );
}