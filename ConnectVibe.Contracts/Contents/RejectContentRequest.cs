using Fliq.Contracts.Enums;

namespace Fliq.Contracts.Contents
{
    public record RejectContentRequest(ContentTypeEnum ContentType, int ContentId, string? RejectionReason);
}
