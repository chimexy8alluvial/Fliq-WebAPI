

namespace Fliq.Domain.Entities.Prompts
{
    public class PromptCategory : Record
    {
        public string CategoryName { get; set; } = default!;

        public bool IsSystemGenerated { get; set; } = true;
        public ICollection<PromptQuestion> PromptQuestions { get; set; } = new List<PromptQuestion>();
    }
}
