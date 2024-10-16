namespace Fliq.Contracts.Authentication;

public record AuthenticationResponse(
    int Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string Token
    );



