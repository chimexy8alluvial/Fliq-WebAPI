

namespace Fliq.Application.Users.Common
{
    public record GetRecentUserFeatureActivityResult(int UserId, string Feature, DateTime LastActiveAt);
}
