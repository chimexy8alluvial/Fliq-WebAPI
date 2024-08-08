using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Application.Authentication.Common
{
    public record RegistrationResult
     (
       User user,
       string otp
    );
}
