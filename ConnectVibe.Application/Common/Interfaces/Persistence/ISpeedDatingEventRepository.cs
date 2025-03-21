using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Enums;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public interface ISpeedDatingEventRepository
    {
        Task<SpeedDatingEvent?> GetByIdAsync(int id);
        Task<IEnumerable<SpeedDatingEvent>> GetAllAsync();
        Task<IEnumerable<SpeedDatingEvent>> GetByCategoryAsync(SpeedDatingCategory category);
        Task AddAsync(SpeedDatingEvent speedDate);
        Task UpdateAsync(SpeedDatingEvent speedDate);
        Task DeleteAsync(SpeedDatingEvent speedDate);
        Task<int> GetSpeedDateCountAsync();
    }
}