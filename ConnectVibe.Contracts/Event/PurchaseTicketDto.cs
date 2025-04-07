namespace Fliq.Contracts.Event
{
    public record PurchaseTicketDto(
       int EventId,
       int PaymentId,
       Dictionary<string, int> TicketQuantities
   );
}