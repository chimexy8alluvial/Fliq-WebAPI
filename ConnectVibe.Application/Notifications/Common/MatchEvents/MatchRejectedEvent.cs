

namespace Fliq.Application.Notifications.Common.MatchEvents
{
    public record MatchRejectedEvent : NotificationEvent
    {

        public MatchRejectedEvent(int userId)
            : base(userId, "Match Rejected", "You have rejected a match.")
        {
        }
    }
}
