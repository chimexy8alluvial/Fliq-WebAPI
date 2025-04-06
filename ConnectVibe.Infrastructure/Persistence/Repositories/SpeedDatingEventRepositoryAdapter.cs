

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SpeedDatingEventRepositoryAdapter : IGenericRepository<SpeedDatingEvent>
    {
        private readonly ISpeedDatingEventRepository _speedDatingEventRepository;

        public SpeedDatingEventRepositoryAdapter(ISpeedDatingEventRepository speedDatingEventRepository)
        {
            _speedDatingEventRepository = speedDatingEventRepository;
        }

        public async Task<SpeedDatingEvent?> GetByIdAsync(int id)
        {
            return await _speedDatingEventRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(SpeedDatingEvent speedDatingEvent)
        {
            await _speedDatingEventRepository.UpdateAsync(speedDatingEvent);
        }
    }
}
