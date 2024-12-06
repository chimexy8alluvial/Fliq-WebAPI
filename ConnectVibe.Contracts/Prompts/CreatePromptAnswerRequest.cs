using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Prompts
{
    public record CreatePromptAnswerRequest(
        int PromptQuestionId,
        //string? CustomPromptId = null,
        string? TextAnswer = null,
        IFormFile? VoiceNote = null,
        IFormFile? VideoClip = null
        );
}
