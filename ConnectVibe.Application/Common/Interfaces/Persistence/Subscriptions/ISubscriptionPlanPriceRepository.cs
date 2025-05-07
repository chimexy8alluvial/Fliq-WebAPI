using Fliq.Domain.Entities.Subscriptions;

namespace Fliq.Application.Common.Interfaces.Persistence.Subscriptions
{
    public interface ISubscriptionPlanPriceRepository
    {
        Task AddAsync(SubscriptionPlanPrice subscriptionPlanPrice);

        Task<SubscriptionPlanPrice?> GetByIdAsync(int subscriptionPlanPriceId);

        Task Update(SubscriptionPlanPrice subscriptionPlanPrice);
    }
}
