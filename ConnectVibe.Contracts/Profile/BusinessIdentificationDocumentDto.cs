
using Fliq.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Profile
{
    public class BusinessIdentificationDocumentDto
    {
        public int BusinessIdentificationDocumentTypeId { get; set; } = default!;
        public BusinessIdentificationDocumentType? BusinessIdentificationDocumentType = default!;
        public IFormFile? BusinessIdentificationDocumentFront { get; set; } = default!;
        public IFormFile? BusinessIdentificationDocumentBack { get; set; } = default!;
    }
}
