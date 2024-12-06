using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Prompts.Common
{
    public record CustomPromptRequestDto(string QuestionText, 
        string? TextResponse,
        IFormFile? VoiceNote,
        IFormFile? VideoClip);
}
