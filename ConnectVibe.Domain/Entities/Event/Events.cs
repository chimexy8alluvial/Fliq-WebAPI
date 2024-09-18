using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class Events
    {
        public int Id { get; set; }
        public EventType EventType { get; set; }
        public string EventTitle { get; set; } = default!;
        public string EventDescription { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Location Location { get; set; } = default!;
        public int Capacity { get; set; }
        public List<EventMediaa> Media { get; set; } = default!;
        public string StartAge { get; set; } = default!;
        public string EndAge { get; set; } = default!;
        public string EventCategory { get; set; } = default!;
        public bool SponsoredEvent { get; set; } = default!;
        public SponsoredEventDetail SponsoredEventDetail { get; set; } = default!;
        public EventCriteria EventCriteria { get; set; } = default!;
        public TicketType TicketType { get; set; } = default!;
        public int UserId { get; set; } = default!;
        public UserProfile User { get; set; } = default!;
    }
}
