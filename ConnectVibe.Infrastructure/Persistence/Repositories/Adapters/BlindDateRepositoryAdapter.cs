using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;

namespace Fliq.Infrastructure.Persistence.Repositories.Adapters
{
    public class BlindDateRepositoryAdapter : IGenericRepository<BlindDate>
    {
        private readonly IBlindDateRepository _blindDateRepository;

        public BlindDateRepositoryAdapter(IBlindDateRepository blindDateRepository)
        {
            _blindDateRepository = blindDateRepository;
        }

        public async Task<BlindDate?> GetByIdAsync(int id)
        {
            return await _blindDateRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(BlindDate blindDate)
        {
            await _blindDateRepository.UpdateAsync(blindDate);
        }

        public async Task<int> CountAsync()
        {
            return await _blindDateRepository.CountAsync();
        }

        public async Task<int> FlaggedCountAsync()
        {
            return await _blindDateRepository.FlaggedCountAsync();
        }
    }
}
