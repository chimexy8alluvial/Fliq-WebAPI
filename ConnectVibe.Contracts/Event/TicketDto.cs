namespace Fliq.Contracts.Event
{
    public record TicketDto
    {
        public string TicketName { get; set; } = default!;
        public string TicketDescription { get; set; } = default!;
        public DateTime OpensOn { get; set; } = default!;
        public DateTime ClosesOn { get; set; } = default!;
        public string TimeZone { get; set; } = default!;
        public string TicketTypes { get; set; } = default!;
        public int CurrencyId { get; set; } = default!;
        public double Amount { get; set; }
        public string MaximumLimit { get; set; } = default!;
        public List<DiscountDto>? Discounts { get; set; } = default!;
    };
}