using Fliq.Domain.Entities.Interfaces;
using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Prompts
{
    public class PromptQuestion : Record, IApprovableContent
    {
        public string QuestionText { get; set; } = default!;
        public bool IsSystemGenerated { get; set; }  // Indicates if the prompt question is predefined by the system or user-created
       
        //Track creator
        public bool CreatorIsAdmin { get; set; }
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;

        // Category this question belongs to
        public int PromptCategoryId { get; set; }
        public PromptCategory PromptCategory { get; set; } = default!;

        // This can be used to track if this prompt is a custom prompt or not
        public string? CustomPromptId { get; set; } // Nullable if it's a standard prompt

        public bool IsFlagged { get; set; }

        //Track approval status
        public ContentCreationStatus ContentCreationStatus { get; set; } = ContentCreationStatus.Pending;
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string? RejectionReason { get; set; }

    }
}
