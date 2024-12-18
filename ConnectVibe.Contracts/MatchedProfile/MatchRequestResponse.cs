using Fliq.Contracts.Profile;

namespace Fliq.Contracts.MatchedProfile
{
    public record MatchRequestResponse
    (
        int Id,
        int RequestedUserId,
        int MatchInitiatorUserId,
        string PictureUrl,
        string Name,
        int? Age,
        int MatchRequestStatus
    );
}
