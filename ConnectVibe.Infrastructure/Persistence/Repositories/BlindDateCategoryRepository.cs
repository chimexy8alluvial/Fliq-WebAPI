
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class BlindDateCategoryRepository : IBlindDateCategoryRepository
    {
        private readonly FliqDbContext _dbContext;

        public BlindDateCategoryRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BlindDateCategory?> GetByIdAsync(int id)
        {
            return await _dbContext.BlindDateCategories
                .Include(c => c.BlindDates)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<BlindDateCategory?> GetByCategoryName(string CategoryName)
        {
            return await _dbContext.BlindDateCategories
                .Include(c => c.BlindDates)
                .FirstOrDefaultAsync(c => c.CategoryName == CategoryName);
        }

        public async Task<IEnumerable<BlindDateCategory>> GetAllAsync()
        {
            return await _dbContext.BlindDateCategories
                .Include(c => c.BlindDates)
                .ToListAsync();
        }

        public async Task AddAsync(BlindDateCategory category)
        {
            await _dbContext.BlindDateCategories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlindDateCategory category)
        {
            _dbContext.BlindDateCategories.Update(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(BlindDateCategory category)
        {
            _dbContext.BlindDateCategories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

    }
}
