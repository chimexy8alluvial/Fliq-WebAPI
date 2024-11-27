namespace Fliq.Contracts.Event
{
    public record PurchaseTicketDto(
        int PaymentId,
        int TicketId,
        int NumberOfTickets
        );
}