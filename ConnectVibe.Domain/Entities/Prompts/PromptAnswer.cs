using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.Prompts
{
    public class PromptAnswer : Record
    {
        public int PromptQuestionId { get; set; }
        public PromptQuestion PromptQuestion { get; set; } = default!;
        public string AnswerText { get; set; } = default!;
        public string VoiceNoteUrl { get; set; } = default!;
        public string VideoClipUrl { get; set; } = default!;
        public int UserId { get; set; }
    }
}
