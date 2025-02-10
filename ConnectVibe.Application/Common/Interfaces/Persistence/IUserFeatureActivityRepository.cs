

using Fliq.Domain.Entities.UserFeatureActivities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IUserFeatureActivityRepository
    {
        Task AddAsync(UserFeatureActivity userActivity);
        Task<UserFeatureActivity?> GetUserFeatureActivityAsync(int userId, string feature);
        Task<List<UserFeatureActivity>> GetInactiveFeatureUsersAsync(DateTime thresholdDate);
    }
}
