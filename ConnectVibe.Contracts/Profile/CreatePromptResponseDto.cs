using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Profile
{
    public record CreatePromptResponseDto(
 
        int? PromptQuestionId,
        string? CustomPromptQuestionText,
        string? TextResponse,
        IFormFile? VoiceNote,
        IFormFile? VideoClip,
               int CategoryId,
        bool IsCustomPrompt
        );
}