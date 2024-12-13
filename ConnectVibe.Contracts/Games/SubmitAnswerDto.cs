namespace Fliq.Contracts.Games
{
    public record SubmitAnswerDto(
    int GameSessionId,
    int UserId,
    int QuestionId,
    string Answer
);
}