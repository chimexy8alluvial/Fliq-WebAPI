using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Event;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IUserRepository _userRepository;

        public EventRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _userRepository = userRepository;
        }

        public void Add(Events createEvent)
        {
            if (createEvent != null)
            {
                _dbContext.Add(createEvent);
            }
            _dbContext.SaveChanges();
        }

        public void Update(Events request)
        {
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        public Events? GetEventById(int id)
        {
            var result = _dbContext.Events.SingleOrDefault(p => p.Id == id);
            return result;
        }

        public List<Events> GetAllEvents()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                //Highlight this
                const string SQL = "SELECT * FROM Events WHERE EventTitle IS NOT NULL";

                // Ensure 'Events' is a class that matches the schema of your 'Events' table
                var results = connection.Query<Events>(SQL);

                return results.ToList();
            }
        }
    }
}