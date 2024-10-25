using Fliq.Domain.Enums;

namespace Fliq.Application.MatchedProfile.Common
{
    public record CreateAcceptMatchResult
    (
        int MatchInitiatorUserId,
        MatchRequestStatus matchRequestStatus
    );
}
