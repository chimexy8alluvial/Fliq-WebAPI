
namespace Fliq.Contracts.Authentication
{
    public record RegisterBusinessResponse
    (
        int Id,
        string Email,
        string BusinessName,
        string BusinessType,
        string Address,
        string PhoneNumber,
        string Otp,
        string CompanyBio,
        int Language
    );
}
