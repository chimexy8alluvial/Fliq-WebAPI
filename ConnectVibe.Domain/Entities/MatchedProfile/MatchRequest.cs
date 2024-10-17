using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.MatchedProfile
{
    public class MatchRequest : Record
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MatchInitiatorUserId { get; set; }
        public string PictureUrl { get; set; } = default!;
        public string Name { get; set; } = default!;
        public MatchRequestStatus matchRequestStatus { get; set; }
    }
}
