
using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories.Subscriptions
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        private readonly FliqDbContext _dbContext;

        public UserSubscriptionRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(UserSubscription userSubscription)
        {
            await _dbContext.UserSubscriptions.AddAsync(userSubscription);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserSubscription?> GetByIdAsync(int userSubscriptionId)
        {
            return await _dbContext.UserSubscriptions
                 .Include(c => c.SubscriptionPlan)
                 .FirstOrDefaultAsync(c => c.Id == userSubscriptionId);
        }

        public async Task<UserSubscription?> GetActiveSubscription(int userId, int planId)
        {
            return await _dbContext.UserSubscriptions
                 .Include(c => c.SubscriptionPlan)
                 .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive == true && c.SubscriptionPlanId == planId);
        }

        public async Task Update(UserSubscription userSubscription)
        {
            _dbContext.UserSubscriptions.Update(userSubscription);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeactivateAllForUserAsync(int userId)
        {
            var activeSubscriptions = await _dbContext.UserSubscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            foreach (var sub in activeSubscriptions)
            {
                sub.IsActive = false;
                sub.Status = SubscriptionStatus.Expired;
            }

            if (activeSubscriptions.Any())
            {
                _dbContext.UserSubscriptions.UpdateRange(activeSubscriptions);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
