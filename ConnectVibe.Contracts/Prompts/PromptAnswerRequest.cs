using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Prompts
{
    public record PromptAnswerRequest(
        string? AnswerText,
        IFormFile? VoiceNote,
        IFormFile? VideoClip);
}
