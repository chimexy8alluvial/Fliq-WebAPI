namespace ConnectVibe.Contracts.Authentication
{
    public record SendPasswordOTPRequest
    (
        string Email,
        string Otp
    );
}
