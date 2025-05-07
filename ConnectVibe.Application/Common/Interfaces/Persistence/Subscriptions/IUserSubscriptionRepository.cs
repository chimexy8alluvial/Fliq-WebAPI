using Fliq.Domain.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Application.Common.Interfaces.Persistence.Subscriptions
{
    public interface IUserSubscriptionRepository
    {
        Task AddAsync(UserSubscription userSubscription);

        Task<UserSubscription?> GetByIdAsync(int userSubscription);
        Task<UserSubscription?> GetActiveSubscription(int userId, int planId);

        Task Update(UserSubscription userSubscription);
        Task DeactivateAllForUserAsync(int userId);
    }
}
