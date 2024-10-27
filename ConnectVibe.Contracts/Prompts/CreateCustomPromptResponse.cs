

namespace Fliq.Contracts.Prompts
{
    public record CreateCustomPromptResponse(
        int PromptQuestionId,
        string CustomPromptId,
        int PromptAnswerId,
        bool Success
        );
}
