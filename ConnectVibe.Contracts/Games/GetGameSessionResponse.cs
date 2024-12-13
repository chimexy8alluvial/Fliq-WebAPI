namespace Fliq.Contracts.Games
{
    public record GetGameSessionResponse(
     int SessionId,
     int GameId,
     string CurrentQuestion,
     int CurrentTurn,
     int Status
 );
}