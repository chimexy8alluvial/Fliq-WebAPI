
namespace Fliq.Application.Notifications.Common.EventCreatedEvents
{
    public record EventCreatedEvent : NotificationEvent
    {
        public int EventId { get; }
        public int OrganizerId { get; }
        public string OrganizerName { get; }
        public string? OrganizerImageUrl { get; }
        public IEnumerable<int> InviteeIds { get; }

        public EventCreatedEvent(
            int userId,
            int eventId,
            int organizerId,
            string organizerName,
            IEnumerable<int> inviteeIds,
            string title,
            string message,
            string? organizerImageUrl = null,
            string? actionUrl = null,
            string? buttonText = null)
            : base(userId, title, message)
        {
            EventId = eventId;
            OrganizerId = organizerId;
            OrganizerName = organizerName;
            InviteeIds = inviteeIds;
            ImageUrl = organizerImageUrl;
            ActionUrl = actionUrl;
            ButtonText = buttonText ?? "View Event";
        }
    }
}
