using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SpeedDatingEventRepository : ISpeedDatingEventRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public SpeedDatingEventRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public async Task<SpeedDatingEvent?> GetByIdAsync(int id)
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<SpeedDatingEvent>> GetAllAsync()
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task<IEnumerable<SpeedDatingEvent>> GetSpeedDatesForAdmin(int pageSize, int pageNumber, int? creationStatus)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_SpeedDateListForAdmin";

                var parameter = new
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    CreationStatus = creationStatus
                };

                var speedDates = await connection.QueryAsync<SpeedDatingEvent>(sql, parameter, commandType: CommandType.StoredProcedure);
                return speedDates.ToList();
            }

        }

        public async Task<IEnumerable<SpeedDatingEvent>> GetByCategoryAsync(SpeedDatingCategory category)
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => b.Category == category && !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task AddAsync(SpeedDatingEvent speedDating)
        {
            if (speedDating.Id > 0)
            {
                _dbContext.SpeedDatingEvents.Update(speedDating);
            }
            else
            {
                await _dbContext.SpeedDatingEvents.AddAsync(speedDating);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(SpeedDatingEvent speedDate)
        {
            _dbContext.SpeedDatingEvents.Update(speedDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(SpeedDatingEvent speedDate)
        {
            speedDate.IsDeleted = true;
            _dbContext.SpeedDatingEvents.Update(speedDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {

            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountSpeedDates", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> FlaggedCountAsync()
        {

            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountFlaggedEvents", commandType: CommandType.StoredProcedure);
                return count;
            }
        }
    }
}
