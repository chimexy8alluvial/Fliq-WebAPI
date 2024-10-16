using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ISubscriptionRepository
    {
        void Add(Subscription subscription);

        Subscription? GetSubscriptionByUserIdAndProductIdAsync(int userId, string productId);

        void Update(Subscription subscription);
    }
}