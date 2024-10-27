

namespace Fliq.Application.Prompts.Common
{
    public record CreatePromptAnswerResult(
        int QuestionId,
        int AnswerId,
        bool IsAnswered
        );
}
