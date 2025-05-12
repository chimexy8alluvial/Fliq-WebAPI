using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Domain.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories.Subscriptions
{
    public class SubscriptionPlanPriceRepository : ISubscriptionPlanPriceRepository
    {
        private readonly FliqDbContext _dbContext;

        public SubscriptionPlanPriceRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SubscriptionPlanPrice subscriptionPlanPrice)
        {
            await _dbContext.SubscriptionPlanPrices.AddAsync(subscriptionPlanPrice);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SubscriptionPlanPrice?> GetByIdAsync(int subscriptionPlanPriceId)
        {
            return await _dbContext.SubscriptionPlanPrices
                 .Include(c => c.SubscriptionPlan)
                 .FirstOrDefaultAsync(c => c.Id == subscriptionPlanPriceId);
        }

        public async Task Update(SubscriptionPlanPrice subscriptionPlanPrice)
        {
            _dbContext.SubscriptionPlanPrices.Update(subscriptionPlanPrice);
            await _dbContext.SaveChangesAsync();
        }
    }
}
