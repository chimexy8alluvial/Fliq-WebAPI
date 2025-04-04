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
            public static Error TicketAlreadyRefunded => Error.Failure(
             code: "Ticket.TicketAlreadyRefunded",
             description: "Ticket already refunded."); 
            public static Error TicketAlreadySoldOut => Error.Failure(
             code: "Ticket.TicketAlreadySoldOut",
             description: "Ticket already soldout.");
        }
    }
}