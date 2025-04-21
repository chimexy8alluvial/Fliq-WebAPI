namespace Fliq.Contracts.Authentication
{
    public record RegisterResponse(
    int Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string BusinessName,
    string BusinessType,
    string BusinessAddress,
    string CompanyBio,
    string Otp,
    string Language
    );
}