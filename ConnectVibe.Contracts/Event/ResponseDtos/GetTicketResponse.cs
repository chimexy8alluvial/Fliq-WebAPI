namespace Fliq.Contracts.Event.ResponseDtos
{
    public class GetTicketResponse
    {
        public string? TicketName { get; set; } = default!;
        public int TicketType { get; set; } = default!;
        public string TicketDescription { get; set; } = default!;
        public DateTime EventDate { get; set; }
        public string Currency { get; set; } = default!;
        public decimal Amount { get; set; } = default!;
        public string MaximumLimit { get; set; } = default!;
        public bool SoldOut { get; set; } = default!;
        public List<DiscountDto>? Discounts { get; set; } = default!;
        public int EventId { get; set; }
    }
}