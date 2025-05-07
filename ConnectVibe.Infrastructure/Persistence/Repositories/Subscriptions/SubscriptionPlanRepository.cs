using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Domain.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories.Subscriptions
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly FliqDbContext _dbContext;

        public SubscriptionPlanRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SubscriptionPlan subscriptionPlan)
        {
            await _dbContext.SubscriptionPlans.AddAsync(subscriptionPlan);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int subscriptionPlanId)
        {
            return await _dbContext.SubscriptionPlans
                 .FirstOrDefaultAsync(c => c.Id == subscriptionPlanId);
        }

        public async Task<SubscriptionPlan?> GetByProductIdAsync(string productId)
        {
            return await _dbContext.SubscriptionPlans
                 .FirstOrDefaultAsync(c => c.ProductId == productId);
        }

        public async Task<SubscriptionPlan?> GetByName(string subscriptionPlanName)
        {
            return await _dbContext.SubscriptionPlans
                .FirstOrDefaultAsync(c => c.Name == subscriptionPlanName);
        }

        public async Task Update(SubscriptionPlan subscriptionPlan)
        {
            _dbContext.SubscriptionPlans.Update(subscriptionPlan);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<SubscriptionPlan>> GetAllAsync()
        {
            return await _dbContext.SubscriptionPlans.ToListAsync();
        }
    }
}
