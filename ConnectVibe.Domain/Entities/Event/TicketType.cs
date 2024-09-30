using ConnectVibe.Domain.Entities.Profile;
namespace Fliq.Domain.Entities.Event
{
    public class TicketType
    {
        public int Id { get; set; }
        public string TicketName { get; set; } = default!;
        public string TicketDescription { get; set; } = default!;
        public string OpensOn { get; set; } = default!;
        public string ClosesOn { get; set; } = default!;
        public string TimeZone { get; set; } = default!;
        public string TicketTypes { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public double Amount { get; set; } = default!;
        public string MaximumLimit { get; set; } = default!;
        public int Discount { get; set; } = default!;
    }
}
