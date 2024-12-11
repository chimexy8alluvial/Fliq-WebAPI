

namespace Fliq.Contracts.Prompts
{
    public record CreatePromptAnswerResponse(
        int PromptQuestionId,
        int PromptAnswerId,
        bool Success,                
        string? CustomPromptId = null
        );
}
