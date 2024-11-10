using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Prompts
{
    public class PromptResponse : Record
    {
        // Foreign key linking this response to a UserProfile
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; } = default!;

        // Foreign key linking this response to a prompt question
        public int PromptQuestionId { get; set; }
        public PromptQuestion PromptQuestion { get; set; } = default!;

        public string ResponseType { get; set; } = nameof(PromptAnswerMediaType.Text);

        public string? Response { get; set; }
        //public string? VoiceNoteUrl { get; set; }
        //public string? VideoClipUrl { get; set; }
    }
}
