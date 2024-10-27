using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Prompts
{
    public class PromptAnswer : Record
    {
        public int UserId { get; set; }
        public int PromptQuestionId { get; set; }

        // Answer options: only one should be populated
        public string? TextAnswer { get; set; }
        public string? VoiceNoteUrl { get; set; }
        public string? VideoClipUrl { get; set; }

        // Track the media type of the current answer
        public PromptAnswerMediaType MediaType { get; set; }

        // Navigation Properties
        public User User { get; set; } = default!;
        public PromptQuestion PromptQuestion { get; set; } = default!;

    }
}
