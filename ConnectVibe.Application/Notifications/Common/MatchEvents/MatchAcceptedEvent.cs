

namespace Fliq.Application.Notifications.Common.MatchEvents
{
    public record MatchAcceptedEvent : NotificationEvent
    {
        public int MatchInitiatorUserId { get; }  // The user who initiated the match

        public MatchAcceptedEvent(int userId, int matchInitiatorUserId)
            : base(userId, "Match Accepted!", "You have a new connection.")
        {
            MatchInitiatorUserId = matchInitiatorUserId;
        }
    }
}
