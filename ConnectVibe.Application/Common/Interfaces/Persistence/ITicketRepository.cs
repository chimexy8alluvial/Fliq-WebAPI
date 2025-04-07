using Fliq.Application.DashBoard.Common;
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

        Task<List<GetEventsTicketsResult>> GetAllEventsTicketsForDashBoardAsync(GetEventsTicketsListRequest request);
        List<EventTicket> GetEventTicketsByIds(List<int> eventTicketIds); // Synchronous
        List<Ticket> GetTicketsByEventId(int eventId);
        List<Ticket> GetTicketsByIds(List<int> ids);
        void UpdateRange(IEnumerable<Ticket> tickets);
        void AddEventTickets(IEnumerable<EventTicket> eventTickets);

        Task<int> GetRegularTicketCountAsync(int eventId);
        Task<int> GetVipTicketCountAsync(int eventId);
        Task<int> GetVVipTicketCountAsync(int eventId);
        Task<int> GetOtherTicketCountAsync(int eventId);
        Task<int> GetTotalTicketCountAsync(int eventId);
        Task<int> GetMondayTicketCountAsync(int eventId, TicketType? ticketType);
        Task<int> GetTuesdayTicketCountAsync(int eventId, TicketType? ticketType);
        Task<int> GetWednesdayTicketCountAsync(int eventId, TicketType? ticketType);
        Task<int> GetThursdayTicketCountAsync(int eventId, TicketType? ticketType);
        Task<int> GetFridayTicketCountAsync(int eventId, TicketType? ticketType);
        Task<int> GetSaturdayTicketCountAsync(int eventId, TicketType? ticketType);
        Task<int> GetSundayTicketCountAsync(int eventId, TicketType? ticketType);
        Task<decimal> GetEventTicketGrossRevenueAsync(int eventId);
        Task<decimal> GetEventTicketNetRevenueAsync(int eventId);
    }
}