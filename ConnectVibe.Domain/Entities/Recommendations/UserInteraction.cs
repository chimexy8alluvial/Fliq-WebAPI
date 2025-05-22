using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Recommendations
{
    public class UserInteraction : Record
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public InteractionType Type { get; set; } // View, Like, RSVP, Attend

        // The entity being interacted with (one of these will be non-null)
        public int? EventId { get; set; }
        public Events? Event { get; set; }

        public int? BlindDateId { get; set; }
        public BlindDate? BlindDate { get; set; }

        public int? SpeedDatingEventId { get; set; }
        public SpeedDatingEvent? SpeedDatingEvent { get; set; }

        public int? InteractedWithUserId { get; set; }
        public User? InteractedWithUser { get; set; }

        public DateTime InteractionTime { get; set; }
        public double InteractionStrength { get; set; } // 0-1 value for weight
    }
}
