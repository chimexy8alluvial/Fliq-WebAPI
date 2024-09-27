using Fliq.Domain.Entities;

namespace Fliq.Application.Authentication.Common
{
    public record SocialAuthenticationResult
    (
        User user,
       string Token,
       bool IsNewUser
    );
}