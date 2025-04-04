using Fliq.Domain.Entities.DatingEnvironment.BlindDates;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IBlindDateRepository
    {
        Task<BlindDate?> GetByIdAsync(int id);
        Task<IEnumerable<BlindDate>> GetAllAsync();
        Task<IEnumerable<BlindDate>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<BlindDate>> GetBlindDatesForAdmin(int pageSize, int pageNumber, int? creationStatus);
        Task AddAsync(BlindDate blindDate);
        Task UpdateAsync(BlindDate blindDate);
        Task DeleteAsync(BlindDate blindDate);
    }
}
