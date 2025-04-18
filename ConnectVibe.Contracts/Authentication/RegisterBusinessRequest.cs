
using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Authentication
{
    public record RegisterBusinessRequest(
        //int AccountType,
        string Email,
        string Password,
        string BusinessName,
        string BusinessType,
        string Address,
        string PhoneNumber,
        int DocumentTypeId,
        IFormFile BusinessIdentificationDocumentFront,
        IFormFile? BusinessIdentificationDocumentBack,
        string CompanyBio,
        int Language,
        int Theme
    );
}
