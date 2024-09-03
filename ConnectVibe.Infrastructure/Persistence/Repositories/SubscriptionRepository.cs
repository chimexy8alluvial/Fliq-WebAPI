using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Infrastructure.Persistence;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public SubscriptionRepository(ConnectVibeDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(Subscription subscription)
        {
            if (subscription.Id > 0)
            {
                _dbContext.Update(subscription);
            }
            else
            {
                _dbContext.Add(subscription);
            }
            _dbContext.SaveChanges();
        }

        public void Update(Subscription subscription)
        {
            _dbContext.Update(subscription);

            _dbContext.SaveChanges();
        }

        public Subscription? GetSubscriptionByUserIdAndProductIdAsync(int userId, string productId)
        {
            var subscription = _dbContext.Subscriptions.SingleOrDefault(s => s.UserId == userId && s.ProductId == productId);
            return subscription;
        }
    }
}