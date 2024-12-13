namespace Fliq.Contracts.Games
{
    public record SendGameRequestDto(
      int GameId,
      int SenderUserId,   // The user sending the request
      int ReceiverUserId  // The user receiving the request
  );
}