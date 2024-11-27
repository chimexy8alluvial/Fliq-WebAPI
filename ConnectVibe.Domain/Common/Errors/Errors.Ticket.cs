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
        }
    }
}