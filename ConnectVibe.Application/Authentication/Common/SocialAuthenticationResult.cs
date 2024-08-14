using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Application.Authentication.Common
{
    public record SocialAuthenticationResult
    (
        User user,
       string Token,
       bool IsNewUser
    );
}