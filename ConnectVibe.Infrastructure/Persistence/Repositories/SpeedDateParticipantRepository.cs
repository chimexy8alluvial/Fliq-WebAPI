using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SpeedDateParticipantRepository : ISpeedDateParticipantRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public SpeedDateParticipantRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public async Task<SpeedDatingParticipant?> GetByIdAsync(int id)
        {
            return await _dbContext.SpeedDatingParticipanticipants
                .Include(p => p.SpeedDatingEvent)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<SpeedDatingParticipant>> GetByBlindDateIdAsync(int speedDateId)
        {
            return await _dbContext.SpeedDatingParticipanticipants
                .Where(p => p.SpeedDatingEventId == speedDateId)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<SpeedDatingParticipant>> GetAllAsync()
        {
            return await _dbContext.SpeedDatingParticipanticipants
                .Include(p => p.SpeedDatingEvent)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task AddAsync(SpeedDatingParticipant participant)
        {
            await _dbContext.SpeedDatingParticipanticipants.AddAsync(participant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(SpeedDatingParticipant participant)
        {
            _dbContext.SpeedDatingParticipanticipants.Update(participant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(SpeedDatingParticipant participant)
        {
            _dbContext.SpeedDatingParticipanticipants.Remove(participant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SpeedDatingParticipant?> GetByUserAndSpeedDateId(int userId, int speedDateId)
        {
            return await _dbContext.SpeedDatingParticipanticipants
                          .FirstOrDefaultAsync(c => c.UserId == userId && c.SpeedDatingEventId == speedDateId);
        }

        public async Task<int> CountByBlindDateId(int speedDateId)
        {
            return await _dbContext.SpeedDatingParticipanticipants
                         .Where(c => c.SpeedDatingEventId == speedDateId).CountAsync();
        }

        public async Task<SpeedDatingParticipant?> GetCreatorByBlindDateId(int speedDateId)
        {
            return await _dbContext.SpeedDatingParticipanticipants
                          .FirstOrDefaultAsync(c => c.SpeedDatingEventId == speedDateId && c.IsCreator);
        }

    }
}
