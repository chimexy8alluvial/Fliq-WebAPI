namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IBusinessIdentificationDocumentTypeRepository
    {
        Task AddBusinessIdentificationDocumentTypeAsync(Domain.Entities.BusinessIdentificationDocumentType documentType);
        IEnumerable<Domain.Entities.BusinessIdentificationDocumentType> GetAllBusinessIdentificationDocumentTypesAsync();
        Task<Domain.Entities.BusinessIdentificationDocumentType> GetByIdAsync(int id);
        Task<bool> DocumentTypeExists(int documentTypeId);
        Task<bool> IsInUseAsync(int id);
        Task DeleteAsync(int id);
    }
}
