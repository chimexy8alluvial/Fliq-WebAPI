using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Event;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public TicketRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(Ticket ticket)
        {
            if (ticket != null)
            {
                _dbContext.Add(ticket);
            }
            _dbContext.SaveChanges();
        }

        public void Update(Ticket request)
        {
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        public Ticket? GetTicketById(int id)
        {
            var result = _dbContext.Tickets.SingleOrDefault(p => p.Id == id);
            return result;
        }

        public List<Currency> GetCurrenciees()
        {
            var result = _dbContext.Currencies.ToList();
            return result;
        }

        public void AddEventTicket(EventTicket eventTicket)
        {
            if (eventTicket != null)
            {
                _dbContext.Add(eventTicket);
            }
            _dbContext.SaveChanges();
        }

        public void UpdateEventTicket(EventTicket request)
        {
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        public EventTicket? GetEventTicketById(int id)
        {
            var result = _dbContext.EventTickets.SingleOrDefault(p => p.Id == id);
            return result;
        }
    }
}