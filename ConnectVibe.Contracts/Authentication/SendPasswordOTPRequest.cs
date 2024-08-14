namespace ConnectVibe.Contracts.Authentication
{
    public record SendPasswordOTPRequest
    (
        string otp,
        string Email
    );
}
