using Fliq.Domain.Entities.Interfaces;
using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Games
{
    public class Game : Record, IApprovableContent
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool RequiresLevel { get; set; }
        public bool RequiresTheme { get; set; }
        public bool RequiresCategory { get; set; }
        public bool IsFlagged { get; set; }

        //Track approval status
        public ContentCreationStatus ContentCreationStatus { get; set; } = ContentCreationStatus.Pending;
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string? RejectionReason { get; set; }
    }
}