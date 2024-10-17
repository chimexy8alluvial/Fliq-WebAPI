using Fliq.Contracts.Profile;

namespace Fliq.Contracts.MatchedProfile
{
    public record MatchedProfileResponse
    (
        int UserId,
        GenderDto GenderType,
        string FirstName, 
        string LastName,
        string UserName,
        int Age,
        string MatchStatus,
        int RequestingUserId,
        ProfilePhotoDto Photos
    );
}
