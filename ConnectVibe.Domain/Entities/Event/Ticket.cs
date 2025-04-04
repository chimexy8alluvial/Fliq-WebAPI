namespace Fliq.Domain.Entities.Event
{
    public class Ticket : Record
    {
        public string? TicketName { get; set; } = default!;
        public TicketType TicketType { get; set; } = default!;
        public string TicketDescription { get; set; } = default!;
        public DateTime EventDate { get; set; }
        public Currency Currency { get; set; } = default!;
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; } = default!;
        public int MaximumLimit { get; set; } = default!;
        public bool SoldOut { get; set; } = default!;
        public List<Discount>? Discounts { get; set; } = default!;
        public Events Event { get; set; } = default!;
        public int EventId { get; set; }
    }
}