namespace Fliq.Contracts.Event.UpdateDtos
{
    public record UpdateTicketDto(
       int Id,
       string? TicketName,
       int? TicketType,
       string? TicketDescription,
       DateTime? EventDate,
       int CurrencyId,
       decimal? Amount,
       string? MaximumLimit,
       bool? SoldOut,
       List<UpdateDiscountDto>? Discounts,
       int EventId
   );
}