using ConnectVibe.Domain.Entities.Profile;

namespace Fliq.Domain.Entities.Event
{
    public class Events
    {
        public int Id { get; set; }
        public EventType EventType { get; set; }
        //public int? EventDetailId { get; set; }
        public string eventTitle { get; set; } = default!;
        public string eventDescription { get; set; } = default!;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        //public TimeZoneInfo timeZone { get; set; } = default!;
        public Location Location { get; set; } = default!;
        public int capacity { get; set; }
        public string optional { get; set; } = default!;
        public int UserId { get; set; } = default!;
        public List<EventDocument> Docs { get; set; } = default!;
    }

    public enum EventType
    {
        Physical,
        Live
    }



}
