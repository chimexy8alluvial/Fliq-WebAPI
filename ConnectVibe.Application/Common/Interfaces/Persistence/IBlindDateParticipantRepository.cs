using Fliq.Domain.Entities.DatingEnvironment;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IBlindDateParticipantRepository
    {
        Task AddAsync(BlindDateParticipant participant);
        Task DeleteAsync(BlindDateParticipant participant);
        Task<IEnumerable<BlindDateParticipant>> GetAllAsync();
        Task<IEnumerable<BlindDateParticipant>> GetByBlindDateIdAsync(int blindDateId);
        Task<BlindDateParticipant?> GetByIdAsync(int id);
        Task UpdateAsync(BlindDateParticipant participant);
    }
}