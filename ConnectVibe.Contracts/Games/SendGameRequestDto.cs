namespace Fliq.Contracts.Games
{
    public record SendGameRequestDto(
      int GameId,
      int ReceiverUserId,  // The user receiving the request,
      int GameDisconnectionResolutionOption
  );
}