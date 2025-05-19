using Fliq.Contracts.Event.UpdateDtos;

namespace Fliq.Contracts.Event
{
   public record AddTicketResponse
    (int Id,
       string TicketName,
       int TicketType,
       string TicketDescription,
       DateTime EventDate,
       int CurrencyId,
       decimal Amount,
       int MaximumLimit,
       bool SoldOut,
       List<UpdateDiscountDto>? Discounts,
       int EventId);
}
