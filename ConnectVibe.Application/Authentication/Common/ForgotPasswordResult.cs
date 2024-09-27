using Fliq.Domain.Entities;

namespace Fliq.Application.Authentication.Common
{
    public record ForgotPasswordResult
    (
       string otp,
       string email
    );
}
