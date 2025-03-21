

using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class BlindDateRepository : IBlindDateRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public BlindDateRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public async Task<BlindDate?> GetByIdAsync(int id)
        {
            return await _dbContext.BlindDates
                .Where(b => !b.IsDeleted)
                .Include(b => b.BlindDateCategory)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BlindDate>> GetAllAsync()
        {
            return await _dbContext.BlindDates
                .Where(b => !b.IsDeleted)
                .Include(b => b.BlindDateCategory)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlindDate>> GetByCategoryAsync(int categoryId)
        {
            return await _dbContext.BlindDates
                .Where(b => b.CategoryId == categoryId && !b.IsDeleted )
                .Include(b => b.BlindDateCategory)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task AddAsync(BlindDate blindDate)
        {
            if(blindDate.Id > 0)
            {
                _dbContext.BlindDates.Update(blindDate);
            }
            else
            {
                await _dbContext.BlindDates.AddAsync(blindDate);
            }
   
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlindDate blindDate)
        {
            _dbContext.BlindDates.Update(blindDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(BlindDate blindDate)
        {
            blindDate.IsDeleted = true;
            _dbContext.BlindDates.Update(blindDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetBlindDateCountAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_BlindDatingCount", commandType: CommandType.StoredProcedure);
                return count;
            }
        }
    }
}
