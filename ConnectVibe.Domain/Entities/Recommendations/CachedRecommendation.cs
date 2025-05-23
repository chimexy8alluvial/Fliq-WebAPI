using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;

namespace Fliq.Domain.Entities.Recommendations
{
    public class CachedRecommendation : Record
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public string RecommendationType { get; set; } = default!; // "Event", "BlindDate", "SpeedDate", "User"

        public int? EventId { get; set; }
        public Events? Event { get; set; }

        public int? BlindDateId { get; set; }
        public BlindDate? BlindDate { get; set; }

        public int? SpeedDatingEventId { get; set; }
        public SpeedDatingEvent? SpeedDatingEvent { get; set; }

        public int? RecommendedUserId { get; set; }
        public User? RecommendedUser { get; set; }

        public double Score { get; set; } = 0.0;
        public DateTime ComputedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
