using Fliq.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Profile.Common
{
    public class BusinessIdentificationDocumentMapped
    {
        public int BusinessIdentificationDocumentTypeId { get; set; } = default!;
        public IFormFile? BusinessIdentificationDocumentFront { get; set; } = default!;
        public IFormFile? BusinessIdentificationDocumentBack { get; set; } = default!;
    }
}
