using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Ticket
        {
            public static Error TicketNotFound => Error.Failure(
             code: "Ticket.TicketNotFound",
             description: "Ticket not found.");
            public static Error NoTicketsAvailable => Error.Failure(
             code: "Ticket.NoTicketsAvailable",
             description: "No Tickets Available.");
            public static Error TicketAlreadyRefunded => Error.Failure(
             code: "Ticket.TicketAlreadyRefunded",
             description: "Ticket already refunded."); 
            public static Error TicketAlreadySoldOut => Error.Failure(
             code: "Ticket.TicketAlreadySoldOut",
             description: "Ticket already soldout.");
            public static Error InsufficientAvailableTickets => Error.Failure(
             code: "Ticket.InsufficientAvailableTickets",
             description: "Insufficient Available Tickets.");
        }
    }
}