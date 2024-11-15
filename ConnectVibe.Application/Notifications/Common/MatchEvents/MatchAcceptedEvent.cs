

namespace Fliq.Application.Notifications.Common.MatchEvents
{
    public record MatchAcceptedEvent : NotificationEvent
    {
        public int MatchInitiatorUserId { get; }  // The user who initiated the match
        public int AccepterUserId { get; }
        public string? AccepterImageUrl { get; }
        public string? InitiatorImageUrl { get; }


        public MatchAcceptedEvent(int userId, int matchInitiatorUserId, int accepterUserId, string? accepterImageUrl = null, string? initiatorImageUrl = null)
            : base(userId, "Match Accepted!", "Your match request has been accepted.")
        {
            MatchInitiatorUserId = matchInitiatorUserId;
            AccepterUserId = accepterUserId;
            AccepterImageUrl = accepterImageUrl;
            InitiatorImageUrl = initiatorImageUrl;
            ActionUrl = "";
            ButtonText = "View Profile";
        }
    }
}
