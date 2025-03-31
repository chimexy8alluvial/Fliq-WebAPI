

namespace Fliq.Contracts.Users.UserFeatureActivities
{
    public record GetRecentUserFeatureActivityResponse(int UserId, string Feature, DateTime LastActiveAt);

}
