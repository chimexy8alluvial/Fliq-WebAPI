

namespace Fliq.Application.Notifications.Common.MatchEvents
{
    public record MatchRequestEvent : NotificationEvent
    {
        public int AccepterUserId { get; }
        public int InitiatorUserId { get; }
        public string? AccepterImageUrl { get; }
        public string? InitiatorImageUrl { get; }
        public string? InitiatorName { get; }

        public MatchRequestEvent(
            int initiatorUserId,
            int accepterUserId,
            string? accepterImageUrl = null,
            string? initiatorImageUrl = null,
            string? initiatorName = null)
            : base(initiatorUserId, "Match Request Sent", "You have initiated a match request.")
        {
            AccepterUserId = accepterUserId;
            InitiatorImageUrl = initiatorImageUrl;
            AccepterImageUrl = accepterImageUrl;
            InitiatorName = initiatorName;
        }
    }
}
