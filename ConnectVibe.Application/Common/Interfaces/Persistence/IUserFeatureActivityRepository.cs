

using Fliq.Domain.Entities.UserFeatureActivities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IUserFeatureActivityRepository
    {
        Task Add(UserFeatureActivity userActivity);
        Task<UserFeatureActivity?> GetUserFeatureActivity(int userId, string feature);
    }
}
