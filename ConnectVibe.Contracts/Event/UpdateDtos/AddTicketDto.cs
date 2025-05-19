namespace Fliq.Contracts.Event.UpdateDtos
{
    public record AddTicketDto
    (
       string TicketName,
       int TicketType,
       string TicketDescription,
       DateTime EventDate,
       decimal Amount,
       int MaximumLimit,
       bool SoldOut,
       List<UpdateDiscountDto>? Discounts,
       int EventId);
}