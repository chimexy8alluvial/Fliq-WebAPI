using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ISpeedDateParticipantRepository
    {
        Task AddAsync(SpeedDatingParticipant participant);
        Task DeleteAsync(SpeedDatingParticipant participant);
        Task<IEnumerable<SpeedDatingParticipant>> GetAllAsync();
        Task<IEnumerable<SpeedDatingParticipant>> GetByBlindDateIdAsync(int blindDateId);
        Task<SpeedDatingParticipant?>GetByUserAndSpeedDateId(int userId, int blindDateId);
        Task<SpeedDatingParticipant?> GetByIdAsync(int id);
        Task<SpeedDatingParticipant?> GetCreatorByBlindDateId(int blindDateId);
        Task UpdateAsync(SpeedDatingParticipant participant);

       Task<int> CountByBlindDateId(int blindDateId);
    }
}