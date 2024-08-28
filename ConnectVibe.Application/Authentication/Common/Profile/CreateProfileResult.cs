using ConnectVibe.Domain.Entities.Profile;

namespace ConnectVibe.Application.Authentication.Common.Profile
{
    public record CreateProfileResult(
        UserProfile Profile
        );
}