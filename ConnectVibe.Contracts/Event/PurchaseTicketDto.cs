namespace Fliq.Contracts.Event
{
    public record PurchaseTicketDto(
     List<int> TicketIds, // Changed from EventId and TicketQuantities
     int PaymentId
 );
}