namespace Fliq.Contracts.Event
{
    public record PurchaseTicketDto(
        int EventId,

        int PaymentId,
        int TicketId,
        int NumberOfTickets
        );


    public class PurchaseTicketDetails
    {
        public string Email { get; set; }
    }
}