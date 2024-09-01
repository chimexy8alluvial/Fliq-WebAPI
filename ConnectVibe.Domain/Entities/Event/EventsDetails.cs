using ConnectVibe.Domain.Entities.Profile;
namespace Fliq.Domain.Entities.Event
{
    public class EventsDetails
    {
        public int Id { get; set; }
        public string email { get; set; } = default!;
        public string eventTitle { get; set; } = default!;
        public string eventDescription { get; set; } = default!;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public TimeZoneInfo timeZone { get; set; } = default!;
        public Location Location { get; set; } = default!;
        public int capacity { get; set; }
        public string optional { get; set; } = default!;
    }
}
