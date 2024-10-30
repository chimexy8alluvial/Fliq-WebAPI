using Fliq.Domain.Enums;

namespace Fliq.Application.MatchedProfile.Common
{
    public record RejectMatchResult
    (
        int MatchInitiatorUserId,
        MatchRequestStatus matchRequestStatus
    );
}
