using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class BlindDateParticipantRepository : IBlindDateParticipantRepository
    {
        private readonly FliqDbContext _dbContext;

        public BlindDateParticipantRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BlindDateParticipant?> GetByIdAsync(int id)
        {
            return await _dbContext.BlindDatesParticipants
                .Include(p => p.BlindDate)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<BlindDateParticipant>> GetByBlindDateIdAsync(int blindDateId)
        {
            return await _dbContext.BlindDatesParticipants
                .Where(p => p.BlindDateId == blindDateId)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlindDateParticipant>> GetAllAsync()
        {
            return await _dbContext.BlindDatesParticipants
                .Include(p => p.BlindDate)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task AddAsync(BlindDateParticipant participant)
        {
            await _dbContext.BlindDatesParticipants.AddAsync(participant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlindDateParticipant participant)
        {
            _dbContext.BlindDatesParticipants.Update(participant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(BlindDateParticipant participant)
        {
            _dbContext.BlindDatesParticipants.Remove(participant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BlindDateParticipant?> GetByUserAndBlindDateId(int userId, int blindDateId)
        {
            return await _dbContext.BlindDatesParticipants
                          .FirstOrDefaultAsync(c => c.UserId == userId && c.BlindDateId == blindDateId);
        }

        public async Task<int> CountByBlindDateId(int blindDateId)
        {
            return await _dbContext.BlindDatesParticipants
                         .Where(c => c.BlindDateId == blindDateId).CountAsync();
        }

        public async Task<BlindDateParticipant?> GetCreatorByBlindDateId(int blindDateId)
        {
            return await _dbContext.BlindDatesParticipants
                          .FirstOrDefaultAsync(c => c.BlindDateId == blindDateId && c.IsCreator);
        }
    }
}
