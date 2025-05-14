
using Fliq.Domain.Entities;

namespace Fliq.Contracts.Profile
{
    public record BusinessIdentificationDocumentResponse
    (
        int BusinessIdentificationDocumentTypeId,
        BusinessIdentificationDocumentType? BusinessIdentificationDocumentType,
        string BusinessIdentificationDocumentFront,
        string BusinessIdentificationDocumentBack
    );
}
