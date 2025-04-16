using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Enums;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public interface ISpeedDatingEventRepository
    {
        Task<SpeedDatingEvent?> GetByIdAsync(int id);
        Task<IEnumerable<SpeedDatingEvent>> GetAllAsync();
        Task<IEnumerable<SpeedDatingEvent>> GetSpeedDatesForAdmin(int pageSize, int pageNumber, int? creationStatus);
        Task<IEnumerable<SpeedDatingEvent>> GetByCategoryAsync(SpeedDatingCategory category);
        Task AddAsync(SpeedDatingEvent speedDate);
        Task UpdateAsync(SpeedDatingEvent speedDate);
        Task DeleteAsync(SpeedDatingEvent speedDate);
        Task<int> CountAsync();
        Task<int> FlaggedCountAsync();
    }
}