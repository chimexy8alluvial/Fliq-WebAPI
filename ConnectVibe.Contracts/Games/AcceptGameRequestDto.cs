namespace Fliq.Contracts.Games
{
    public record AcceptGameRequestDto(
     int GameId,
     int SessionId,
     int UserId,
     bool IsAccepted
 );
}