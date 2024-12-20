namespace Fliq.Contracts.Event
{
    public record TicketDto
    {
        public string TicketName { get; set; } = default!;
        public string TicketDescription { get; set; } = default!;
        public int TicketType { get; set; } = default!;
        public int CurrencyId { get; set; } = default!;
        public decimal Amount { get; set; }
        public int MaximumLimit { get; set; } = default!;
        public bool SoldOut { get; set; }
        public List<DiscountDto>? Discounts { get; set; } = default!;
    };
}