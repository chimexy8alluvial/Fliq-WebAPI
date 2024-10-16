namespace Fliq.Contracts.Authentication
{
    public record SocialAuthenticationResponse
    (
     int Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string Token,
    bool IsNewUser
    );
}