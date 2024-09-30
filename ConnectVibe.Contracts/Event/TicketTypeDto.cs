using ConnectVibe.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record TicketTypeDto
    {
        public string TicketName { get; set; } = default!;
        public string TicketDescription { get; set; } = default!;
        public string OpensOn { get; set; } = default!;
        public string ClosesOn { get; set; } = default!;
        public string TimeZone { get; set; } = default!;
        public string TicketTypes { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public double Amount { get; set; }
        public string MaximumLimit { get; set; } = default!;
        public int Discount { get; set; } = default!;
    };
}
