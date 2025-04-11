using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);

        void Update(Ticket request);

        Ticket? GetTicketById(int id);

        List<Currency> GetCurrenciees();

        void AddEventTicket(EventTicket eventTicket);

        void UpdateEventTicket(EventTicket request);

        EventTicket? GetEventTicketById(int id);

        Task<List<Ticket>> GetTicketsByIdsAsync(List<int> ids);
        Task UpdateRangeAsync(IEnumerable<Ticket> tickets); 
        Task AddEventTicketsAsync(IEnumerable<EventTicket> eventTickets);
    }
}