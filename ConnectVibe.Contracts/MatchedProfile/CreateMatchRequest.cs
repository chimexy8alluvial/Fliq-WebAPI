using Fliq.Contracts.Profile;

namespace Fliq.Contracts.MatchedProfile
{
    public  record CreateMatchRequest(
        int UserId,
        int? MatchInitiatorUserId
     );
}
