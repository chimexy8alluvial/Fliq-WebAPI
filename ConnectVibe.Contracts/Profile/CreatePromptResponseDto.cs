using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Profile
{
    public record CreatePromptResponseDto(int? PromptQuestionId,
        int CategoryId,
        string? CustomPromptQuestionText,
        string? TextResponse,
        IFormFile? VoiceNote,
        IFormFile? VideoClip,
        bool IsCustomPrompt);
}