
namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task UpdateAsync(T entity);
        Task<int> CountAsync();
    }
}
