using Fliq.Domain.Entities;

namespace Fliq.Application.Authentication.Common
{
    public record ValidatePasswordOTPResult
    (
        User user,
        string otp
    );
}
