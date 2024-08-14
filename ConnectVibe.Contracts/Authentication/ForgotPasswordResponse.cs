namespace ConnectVibe.Contracts.Authentication
{
    public record ForgotPasswordResponse(
           int Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string Token
    );
}
