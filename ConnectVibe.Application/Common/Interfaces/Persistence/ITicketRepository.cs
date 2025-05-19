using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);

        void Update(Ticket request);

        Ticket? GetTicketById(int id);

        List<Fliq.Domain.Entities.Event.Currency> GetCurrenciees();

        void AddEventTicket(EventTicket eventTicket);

        void UpdateEventTicket(EventTicket request);

        EventTicket? GetEventTicketById(int id);

        Task<List<GetEventsTicketsResult>> GetAllEventsTicketsForDashBoardAsync(GetEventsTicketsListRequest request);
        List<EventTicket> GetEventTicketsByIds(List<int> eventTicketIds); 
        List<Ticket> GetTicketsByEventId(int eventId);
        List<Ticket> GetTicketsByIds(List<int> ids);
        void UpdateRange(IEnumerable<Ticket> tickets);
        void AddEventTickets(IEnumerable<EventTicket> eventTickets);

        Task<int> GetRegularTicketCountAsync(int eventId);
        Task<int> GetVipTicketCountAsync(int eventId);
        Task<int> GetVVipTicketCountAsync(int eventId);
        Task<int> GetOtherTicketCountAsync(int eventId);
        Task<int> GetTotalTicketCountAsync(int eventId);
        Task<Dictionary<DayOfWeek, int>> GetWeeklyTicketCountAsync(int eventId, DateTime? startDate, DateTime? endDate, TicketType? ticketType);
        Task<decimal> GetEventTicketGrossRevenueAsync(int eventId);
        Task<decimal> GetEventTicketNetRevenueAsync(int eventId);
    }
}