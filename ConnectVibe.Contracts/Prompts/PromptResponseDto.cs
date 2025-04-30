using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Prompts
{
    public record PromptResponseDto(
      
        int PromptQuestionId,
        string? CustomPromptQuestionText,
        string? TextResponse,
        IFormFile? VoiceNote,
        IFormFile? VideoClip,
          int CategoryId,
        bool IsCustomPrompt
        );

    public record ReadPromptResponseDto(

    int? PromptQuestionId,
    string? CustomPromptQuestionText,
    string? Response,
    int? CategoryId,
    bool? IsCustomPrompt
    );
}
