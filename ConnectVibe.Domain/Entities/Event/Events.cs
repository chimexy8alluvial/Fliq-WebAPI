﻿using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Domain.Entities.Event
{
    public class Events : Record
    {
        public EventType EventType { get; set; }
        public string EventTitle { get; set; } = default!;
        public string EventDescription { get; set; } = default!;
        public EventCategory EventCategory { get; set; }
        public EventStatus Status { get; set; }
        public TicketSales TicketSales { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Location Location { get; set; } = default!;
        public int Capacity { get; set; } //Original Capacity
        public List<int> OccupiedSeats { get; set; } = new List<int>(); // To track assigned seat numbers
        public List<EventMedia> Media { get; set; } = new List<EventMedia>();
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public bool SponsoredEvent { get; set; } = default!;
        public bool IsFlagged { get; set; }
        public SponsoredEventDetail? SponsoredEventDetail { get; set; } = default!;
        public EventCriteria EventCriteria { get; set; } = default!;
        public List<Ticket>? Tickets { get; set; } = default!; //Ticket Types
        public int UserId { get; set; } = default!;
        public EventPaymentDetail? EventPaymentDetail { get; set; } = default!;
        public bool InviteesException { get; set; } = default!;
        public List<EventReview> Reviews { get; set; } = new List<EventReview>();
    }
}