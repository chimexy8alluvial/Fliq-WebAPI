using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Interfaces;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class Events : Record, IApprovableContent
    {
        public EventType EventType { get; set; }
        public string EventTitle { get; set; } = default!;
        public string EventDescription { get; set; } = default!;
        public EventCategory EventCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Location Location { get; set; } = default!;
        public int Capacity { get; set; } //Original Capacity
        public List<int> OccupiedSeats { get; set; } = new List<int>(); // To track assigned seat numbers
        public List<EventMedia> Media { get; set; } = new List<EventMedia>();
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public bool SponsoredEvent { get; set; } = default!;
        public SponsoredEventDetail? SponsoredEventDetail { get; set; } = default!;
        public EventCriteria EventCriteria { get; set; } = default!;
        public List<Ticket>? Tickets { get; set; } = default!;

        // Track the creator
        public int UserId { get; set; } = default!;
        public bool CreatorIsAdmin { get; set; }
        public User CreatedByUser { get; set; } = default!;

        public EventPaymentDetail? EventPaymentDetail { get; set; } = default!;
        public bool InviteesException { get; set; } = default!;
        public List<EventReview> Reviews { get; set; } = new List<EventReview>();

        //Track approval status
        public ContentCreationStatus ContentCreationStatus { get; set; } = ContentCreationStatus.Pending;
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string? RejectionReason { get; set; }
    }
}