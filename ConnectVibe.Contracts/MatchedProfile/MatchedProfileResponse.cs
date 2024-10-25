using Fliq.Contracts.Profile;

namespace Fliq.Contracts.MatchedProfile
{
    public record MatchedProfileResponse
    (
        int Id,
        int UserId,
        int MatchInitiatorUserId,
        string PictureUrl,
        string Name,
        int? Age,
        int matchRequestStatus
    );
}
