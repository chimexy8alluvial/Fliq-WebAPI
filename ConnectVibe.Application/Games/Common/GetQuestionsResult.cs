namespace Fliq.Application.Games.Common
{
    public record GetQuestionResult(
      int Id,
      int GameId,
      string QuestionText,
      List<string> Options,
      string CorrectAnswer
  );
}