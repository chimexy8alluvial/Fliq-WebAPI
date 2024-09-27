using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Infrastructure.Persistence;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public SubscriptionRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
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