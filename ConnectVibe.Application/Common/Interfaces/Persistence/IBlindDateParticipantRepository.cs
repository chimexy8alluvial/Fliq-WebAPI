using Fliq.Domain.Entities.DatingEnvironment;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IBlindDateParticipantRepository
    {
        Task AddAsync(BlindDateParticipant participant);
        Task DeleteAsync(BlindDateParticipant participant);
        Task<IEnumerable<BlindDateParticipant>> GetAllAsync();
        Task<IEnumerable<BlindDateParticipant>> GetByBlindDateIdAsync(int blindDateId);
        Task<BlindDateParticipant?>GetByUserAndBlindDateId(int userId, int blindDateId);
        Task<BlindDateParticipant?> GetByIdAsync(int id);
        Task UpdateAsync(BlindDateParticipant participant);

       Task<int> CountByBlindDateId(int blindDateId);
    }
}