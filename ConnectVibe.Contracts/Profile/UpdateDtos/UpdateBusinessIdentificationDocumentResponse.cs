
using Fliq.Domain.Entities;

namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateBusinessIdentificationDocumentResponse
    (
        int BusinessIdentificationDocumentTypeId,
        BusinessIdentificationDocumentType? BusinessIdentificationDocumentType,
        string BusinessIdentificationDocumentFront,
        string BusinessIdentificationDocumentBack
    );

}
