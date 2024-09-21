using ConnectVibe.Domain.Entities.Profile;
namespace Fliq.Application.Event.Common
{
    public class TicketTypesMapped
    {
        public string TicketName { get; set; } = default!;
        public string TicketDescription { get; set; } = default!;
        public string OpensOn { get; set; } = default!;
        public string ClosesOn { get; set; } = default!;
        public string TimeZone { get; set; } = default!;
        public Location Location { get; set; } = default!;
        public string TicketTypes { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public double Amount { get; set; }
    }
}
