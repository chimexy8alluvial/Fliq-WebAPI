using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Application.Authentication.Common
{
    public record ForgotPasswordResult
    (
       string otp
    );
}
