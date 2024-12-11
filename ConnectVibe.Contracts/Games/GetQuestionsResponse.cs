namespace Fliq.Contracts.Games
{
    public record GetQuestionResponse(
    int Id,
    string Text,
    List<string> Options
);
}