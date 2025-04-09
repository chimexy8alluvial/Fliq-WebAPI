using Fliq.Domain.Entities.Event;
using Microsoft.EntityFrameworkCore;

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

        List<Ticket> GetTicketsByIds(List<int> ids);
        void UpdateRange(IEnumerable<Ticket> tickets);
        void AddEventTickets(IEnumerable<EventTicket> eventTickets);
    }
}