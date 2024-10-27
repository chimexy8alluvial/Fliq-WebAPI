

namespace Fliq.Domain.Entities.Prompts
{
    public class PromptCategory : Record
    {
        public string CategoryName { get; set; } = default!;
        public ICollection<PromptQuestion> PromptQuestions { get; set; } = new List<PromptQuestion>();
    }
}
