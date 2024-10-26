namespace Fliq.Contracts.Event.UpdateDtos
{
    public record UpdateTicketResponse
    (
        int Id,
       string TicketName,
       int TicketType,
       string TicketDescription,
       DateTime EventDate,
       string Currency,
       decimal Amount,
       string MaximumLimit,
       bool SoldOut,
       List<UpdateDiscountDto>? Discounts,
       int EventId
       );
}