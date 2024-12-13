

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

        public TicketPurchasedEvent(int buyerId, 
            int organizerId, 
            int eventId, 
            int numberOfTickets,
            string eventTitle,
            string title,
            string message,
            string eventDate, 
            string buyerName) : base(buyerId, title, message)
        {
            BuyerId = buyerId;
            OrganizerId = organizerId;
            EventId = eventId;
            NumberOfTickets = numberOfTickets;
            EventTitle = eventTitle;
            EventDate = eventDate;
            BuyerName = buyerName;
        }
           
    }
}
