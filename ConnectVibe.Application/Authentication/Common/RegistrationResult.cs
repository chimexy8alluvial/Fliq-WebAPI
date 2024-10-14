using Fliq.Domain.Entities;

namespace Fliq.Application.Authentication.Common
{
    public record RegistrationResult
     (
       User user,
       string otp
    );
}
