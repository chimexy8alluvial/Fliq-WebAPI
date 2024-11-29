using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Prompts
{
    public record PromptResponseDto(int PromptQuestionId,
        int CategoryId,
        string? CustomPromptQuestionText,
        string? TextResponse,
        IFormFile? VoiceNote,
        IFormFile? VideoClip,
        bool IsCustomPrompt);
}
