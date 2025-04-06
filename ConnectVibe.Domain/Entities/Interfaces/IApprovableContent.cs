using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Interfaces
{
    public interface IApprovableContent
    {
        ContentCreationStatus ContentCreationStatus { get; set; }
        DateTime? ApprovedAt { get; set; }
        int? ApprovedByUserId { get; set; }
        string? RejectionReason { get; set; }
    }
}
