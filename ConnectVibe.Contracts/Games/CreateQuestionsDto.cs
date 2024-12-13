namespace Fliq.Contracts.Games
{
    public record CreateQuestionDto(
     int GameId,
     string Text,
     List<string> Options,
     int CorrectOptionIndex
 );
}