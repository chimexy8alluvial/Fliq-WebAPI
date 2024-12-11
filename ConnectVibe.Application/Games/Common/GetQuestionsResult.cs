namespace Fliq.Application.Games.Common
{
    public record GetQuestionResult(
      int Id,
      string Text,
      List<string> Options
  );
}