
using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace Fliq.Application.Prompts.Common
{
    public record PromptResponseDto(int PromptQuestionId,
        int CategoryId,
        string? CustomPromptQuestionText,
        string? TextResponse,
        IFormFile? VoiceNote,
        IFormFile? VideoClip,
        bool IsCustomPrompt);
}
