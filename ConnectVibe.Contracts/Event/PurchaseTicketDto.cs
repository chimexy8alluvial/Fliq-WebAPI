namespace Fliq.Contracts.Event
{
    public record PurchaseTicketDto(
        int PaymentId,
        int TicketId
        );
}