using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Domain.Entities.Event
{
    public class Events : Record
    {
        public EventType EventType { get; set; }
        public string EventTitle { get; set; } = default!;
        public string EventDescription { get; set; } = default!;
        public EventCategory EventCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Location Location { get; set; } = default!;
        public int Capacity { get; set; }
        public List<EventMedia> Media { get; set; } = default!;
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public bool SponsoredEvent { get; set; } = default!;
        public SponsoredEventDetail? SponsoredEventDetail { get; set; } = default!;
        public EventCriteria EventCriteria { get; set; } = default!;
        public List<Ticket>? Tickets { get; set; } = default!;
        public int UserId { get; set; } = default!;
        public EventPaymentDetail? EventPaymentDetail { get; set; } = default!;
        public bool InviteesException { get; set; } = default!;
    }
}