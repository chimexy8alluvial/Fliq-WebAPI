using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.MatchedProfile.Common
{
    public record CreateMatchListResult
    (
        MatchRequest matchRequest
    );
}
