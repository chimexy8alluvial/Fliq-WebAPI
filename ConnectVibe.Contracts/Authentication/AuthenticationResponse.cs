namespace ConnectVibe.Contracts.Authentication;

public record AuthenticationRequest(
    int Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string Token
    );

