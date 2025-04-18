namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IBusinessDocumentTypeRepository
    {
        Task AddBusinessDocumentTypeAsync(Domain.Entities.BusinessDocumentType documentType);
        IEnumerable<Domain.Entities.BusinessDocumentType> GetAllBusinessDocumentTypesAsync();
        Task<Domain.Entities.BusinessDocumentType> GetByIdAsync(int id);
        Task<bool> IsInUseAsync(int id);
        Task DeleteAsync(int id);
    }
}
