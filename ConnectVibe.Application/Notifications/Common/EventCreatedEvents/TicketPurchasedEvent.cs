namespace Fliq.Application.Notifications.Common.EventCreatedEvents
{
    public record TicketPurchasedEvent : NotificationEvent
    {
        public int BuyerId { get; }
        public int OrganizerId { get; }
        public int EventId { get; }
        public int NumberOfTickets { get; }
        public string EventTitle { get; }
        public string EventDate { get; }
        public string BuyerName { get; }
        public List<TicketDetail>? TicketDetails { get; }
        public int? SpecificUserId { get; }
        public string? SpecificTicketType { get; }
        public string? Email { get; } // New field for email notifications

        // Constructor for individual user notification (UserId-based)
        public TicketPurchasedEvent(
            int userId,
            int buyerId,
            int organizerId,
            int eventId,
            string eventTitle,
            string eventDate,
            string buyerName,
            string ticketType,
            string title,
            string message)
            : base(userId, title, message)
        {
            BuyerId = buyerId;
            OrganizerId = organizerId;
            EventId = eventId;
            NumberOfTickets = 1;
            EventTitle = eventTitle;
            EventDate = eventDate;
            BuyerName = buyerName;
            SpecificUserId = userId;
            SpecificTicketType = ticketType;
            TicketDetails = null;
            Email = null;
        }

        // Constructor for email notification (Email-based)
        public TicketPurchasedEvent(
            string email,
            int buyerId,
            int organizerId,
            int eventId,
            string eventTitle,
            string eventDate,
            string buyerName,
            string ticketType,
            string title,
            string message)
            : base(0, title, message) // UserId = 0 since it’s email-based
        {
            BuyerId = buyerId;
            OrganizerId = organizerId;
            EventId = eventId;
            NumberOfTickets = 1;
            EventTitle = eventTitle;
            EventDate = eventDate;
            BuyerName = buyerName;
            SpecificUserId = null;
            SpecificTicketType = ticketType;
            TicketDetails = null;
            Email = email;
        }

        // Constructor for buyer notification (all details)
        public TicketPurchasedEvent(
            int buyerId,
            int organizerId,
            int eventId,
            int numberOfTickets,
            string eventTitle,
            string eventDate,
            string buyerName,
            List<TicketDetail> ticketDetails,
            string title,
            string message)
            : base(buyerId, title, message)
        {
            BuyerId = buyerId;
            OrganizerId = organizerId;
            EventId = eventId;
            NumberOfTickets = numberOfTickets;
            EventTitle = eventTitle;
            EventDate = eventDate;
            BuyerName = buyerName;
            TicketDetails = ticketDetails;
            SpecificUserId = null;
            SpecificTicketType = null;
            Email = null;
        }
    }

    public record TicketDetail
    {
        public int? UserId { get; init; } // Nullable to match PurchaseTicketDetail
        public string? TicketType { get; init; }
        public string? Email { get; init; } // Added for email-based assignments
    }
}