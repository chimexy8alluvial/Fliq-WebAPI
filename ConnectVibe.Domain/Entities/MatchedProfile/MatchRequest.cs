using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.MatchedProfile
{
    public class MatchRequest : Record
    {
        public int MatchReceiverUserId { get; set; }
        public int MatchInitiatorUserId { get; set; }
        public MatchRequestStatus MatchRequestStatus { get; set; }
    }
} 