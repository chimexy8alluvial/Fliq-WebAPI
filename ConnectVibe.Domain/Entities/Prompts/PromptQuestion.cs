using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.Prompts
{
    public class PromptQuestion : Record
    {
        public string QuestionText { get; set; } = default!;
        public bool IsSystemGenerated { get; set; }
        public int PromptCategoryId { get; set; }
        public PromptCategory PromptCategory { get; set; } = default!;
        public ICollection<PromptAnswer> PromptAnswers { get; set; } = new List<PromptAnswer>();

    }
}
