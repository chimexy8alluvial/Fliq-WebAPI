using Fliq.Contracts.Enums;
using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Prompts
{
    public record CreateCustomPromptRequest(
        string QuestionText,
        int PromptCategoryId,
        PromptAnswerMediaTypeDto MediaType,
        string? TextAnswer = null,
        IFormFile? VoiceNote = null,
        IFormFile? VideoClip = null
        );
}
