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
        public Location Location { get; set; } = default!;
        public List<string> TicketTypes { get; set; } = default!;
        public List<string> Currency { get; set; } = default!;
        public float Amount { get; set; } = default!;
    }
}
