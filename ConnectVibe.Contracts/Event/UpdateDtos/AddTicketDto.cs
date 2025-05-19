namespace Fliq.Contracts.Event.UpdateDtos
{
    public record AddTicketDto
    (
       string TicketName,
       int TicketType,
       string TicketDescription,
       DateTime EventDate,
       decimal Amount,
       string MaximumLimit,
       bool SoldOut,
       List<UpdateDiscountDto>? Discounts,
       int EventId);
}