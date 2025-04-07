
using Fliq.Contracts.Enums;

namespace Fliq.Contracts.Contents
{
    public record ApproveContentRequest(ContentTypeEnum ContentType, int ContentId);
}
