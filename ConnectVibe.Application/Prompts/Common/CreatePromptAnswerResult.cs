

namespace Fliq.Application.Prompts.Common
{
    public record CreatePromptAnswerResult(
        int PromptQuestionId,
        int PromptAnswerId,
        bool IsAnswered,
        string? CustomPromptId = null
        );
}
