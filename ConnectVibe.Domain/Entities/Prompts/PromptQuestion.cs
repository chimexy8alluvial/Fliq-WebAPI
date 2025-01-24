    
namespace Fliq.Domain.Entities.Prompts
{
    public class PromptQuestion : Record
    {
        public string QuestionText { get; set; } = default!;
        public bool IsSystemGenerated { get; set; }  // Indicates if the prompt question is predefined by the system or user-created
        
        // Category this question belongs to
        public int PromptCategoryId { get; set; }
        public PromptCategory PromptCategory { get; set; } = default!;

        // This can be used to track if this prompt is a custom prompt or not
        public string? CustomPromptId { get; set; } // Nullable if it's a standard prompt
    }
}
