namespace Fliq.Contracts.Authentication
{
    public record SendPasswordOTPRequest
    (
        string Email,
        string Otp
    );
}
