using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Infrastructure.Persistence;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Event.Commands.Create;
using Fliq.Domain.Entities.Event;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class EventDetailsRepository : IEventDetailsRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public EventDetailsRepository(ConnectVibeDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(CreateEventDetailsCommand eventsDetails)
        {
            if (eventsDetails != null)
            {
                _dbContext.Add(eventsDetails);
            }
            _dbContext.SaveChanges();
        }

        //public EventsDetails? GetEventDetailsById(int id)
        //{
        //    var eventss = _dbContext.
        //}
    }
}
