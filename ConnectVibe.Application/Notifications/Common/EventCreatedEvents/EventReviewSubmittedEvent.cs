

using ErrorOr;

namespace Fliq.Application.Notifications.Common.EventCreatedEvents
{
    public record EventReviewSubmittedEvent : NotificationEvent
    {
        public int OrganizerId { get; }
        public int EventId { get; }
        public int Rating { get; }
        public string Comments { get; }

        public EventReviewSubmittedEvent(
        int userId,
        int organizerId,
        int eventId,
        int rating,
        string comments,
        string title,
        string message,
        string? actionUrl = null,
        string? buttonText = null
          )
        : base(userId, title, message)
        {
            OrganizerId = organizerId;
            EventId = eventId; 
            Rating = rating;
            Comments = comments;
            ActionUrl = actionUrl;
            ButtonText = buttonText ?? "View Event";
        }
    }
}
