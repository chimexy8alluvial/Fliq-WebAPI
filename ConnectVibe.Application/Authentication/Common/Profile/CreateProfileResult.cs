using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Authentication.Common.Profile
{
    public record CreateProfileResult(
        UserProfile Profile
        );
}