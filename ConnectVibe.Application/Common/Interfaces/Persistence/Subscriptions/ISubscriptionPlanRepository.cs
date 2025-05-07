using Fliq.Domain.Entities.Subscriptions;

namespace Fliq.Application.Common.Interfaces.Persistence.Subscriptions
{
    public interface ISubscriptionPlanRepository
    {
        Task AddAsync(SubscriptionPlan subscriptionPlan);

        Task<SubscriptionPlan?> GetByIdAsync(int subscriptionPlanId);
        Task<SubscriptionPlan?> GetByProductIdAsync(string productId);
        Task<SubscriptionPlan?> GetByName(string subscriptionPlanName);
        Task<List<SubscriptionPlan>> GetAllAsync();
        Task Update(SubscriptionPlan subscriptionPlan);
    }
}
