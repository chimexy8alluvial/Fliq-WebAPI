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
            public static Error DuplicateTicketType => Error.Failure(
             code: "Ticket.DuplicateTicketType",
             description: "A ticket with this type already exists for the specified event.");
        
        public static Error NoTicketsAvailable => Error.Failure(
             code: "Ticket.NoTicketsAvailable",
             description: "No Tickets Available."); 
            public static Error NoTicketsSpecified => Error.Failure(
             code: "Ticket.NoTicketsSpecified",
             description: "No Tickets Specified.");
            public static Error MultipleEventsNotSupported => Error.Failure(
             code: "Ticket.MultipleEventsNotSupported",
             description: "Multiple Events Not Supported.");
            public static Error TicketAlreadyRefunded => Error.Failure(
             code: "Ticket.TicketAlreadyRefunded",
             description: "Ticket already refunded."); 
            public static Error TicketAlreadySoldOut => Error.Failure(
             code: "Ticket.TicketAlreadySoldOut",
             description: "Ticket already soldout."); 
            public static Error TicketSalesAlreadyStopped => Error.Failure(
             code: "Ticket.TicketSalesAlreadyStopped",
             description: "Ticket sales already stopped.");
            public static Error InsufficientAvailableTickets => Error.Failure(
             code: "Ticket.InsufficientAvailableTickets",
             description: "Insufficient Available Tickets.");

            public static Error WeeklyCountFetchFailed(string message) =>Error.Failure(
                code: "Ticket.WeeklyCount.FetchFailed",
                description: $"Failed to fetch weekly ticket counts: {message}");
            public static Error GrossRevenueFetchFailed(string message) => Error.Failure(
                code: "Ticket.GrossRevenue.FetchFailed", 
                description: $"Failed to fetch gross revenue: {message}");
            public static Error NetRevenueFetchFailed(string message) =>
                Error.Failure(code: "Ticket.NetRevenue.FetchFailed",
                    description: $"Failed to fetch net revenue: {message}");
            public static Error OtherCountFetchFailed(string message) =>
                Error.Failure(code: "Ticket.OtherCount.FetchFailed",
                    description: $"Failed to fetch other ticket count: {message}");
            public static Error RegularCountFetchFailed(string message) =>
                Error.Failure(code: "Ticket.RegularCount.FetchFailed",
                    description: $"Failed to fetch regular ticket count: {message}");
            public static Error VipCountFetchFailed(string message) =>
                Error.Failure(code: "Ticket.VipCount.FetchFailed",
                    description: $"Failed to fetch VIP ticket count: {message}");
            public static Error VVipCountFetchFailed(string message) =>
                Error.Failure(code: "Ticket.VVipCount.FetchFailed", 
                    description: $"Failed to fetch VVIP ticket count: {message}"); 
            public static Error GetEventsTicketsFailed(string message) =>
                Error.Failure(code: "GetEventsTicketsFailed", 
                    description: $"Failed to get all events tickets: {message}");
            public static Error GetTotalTicketCountFailed(string message) =>
                Error.Failure(code: "Ticket.GetTotalTicketCount.Failed", 
                    description: $"Failed to get total tickets count: {message}");
        }
    }
}