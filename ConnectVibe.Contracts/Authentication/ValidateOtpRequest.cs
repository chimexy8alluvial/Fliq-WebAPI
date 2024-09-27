namespace Fliq.Contracts.Authentication
{
    public record ValidateOtpRequest(
        string Email,
        string Otp
        );

}
