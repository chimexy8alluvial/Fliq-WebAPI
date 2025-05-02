using Fliq.Domain.Entities.Event;

namespace Fliq.Application.DashBoard.Common
{
    public record RefundTicketResult(List<EventTicket> RefundedTickets);
}
