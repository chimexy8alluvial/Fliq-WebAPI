namespace Fliq.Contracts.Authentication;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string? DisplayName,
    string? BusinessName,
    string? BusinessType,
    string? BusinessAddress,
    string? CompanyBio,
    string? ContactInformation,
    string Email,
    string Password,
    int Language,
    string Theme,
    string? PhoneNumber
    );