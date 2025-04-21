
using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IBusinessIdentificationDocumentRepository
    {
        Task<BusinessIdentificationDocument> GetByIdAsync(int id);
        //Task<IEnumerable<BusinessIdentificationDocument>> GetByUserIdAsync(int userId);
        void Add(BusinessIdentificationDocument businessDocument);
        void Update(BusinessIdentificationDocument businessDocument);
        void Delete(int id);
    }
}
