using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Event;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class EventReviewRepository : IEventReviewRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public EventReviewRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(EventReview eventReview)
        {
            if (eventReview != null)
            {
                _dbContext.Add(eventReview);
            }
            _dbContext.SaveChanges();
        }

        public void Update(EventReview request)
        {
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        public EventReview? GetEventReviewById(int id)
        {
            var result = _dbContext.EventReviews.SingleOrDefault(p => p.Id == id);
            return result;
        }
    }
}