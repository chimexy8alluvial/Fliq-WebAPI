namespace ConnectVibe.Contracts.Authentication
{
    public record ValidatePasswordOTPResponse
    (
        int Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
        string otp
    );
}
