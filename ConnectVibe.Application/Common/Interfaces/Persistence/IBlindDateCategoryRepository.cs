using Fliq.Domain.Entities.DatingEnvironment.BlindDates;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IBlindDateCategoryRepository
    {
        Task<BlindDateCategory?> GetByIdAsync(int id);
        Task<BlindDateCategory?> GetByCategoryName(string CategoryName);
        Task<IEnumerable<BlindDateCategory>> GetAllAsync();
        Task AddAsync(BlindDateCategory category);
        Task UpdateAsync(BlindDateCategory category);
        Task DeleteAsync(BlindDateCategory category);
    }
}
