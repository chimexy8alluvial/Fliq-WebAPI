namespace Fliq.Contracts.Games
{
    public record SubmitAnswerDto(
    int GameSessionId,
    int Player1Score,
    int Player2Score,
     bool Completed
);
}