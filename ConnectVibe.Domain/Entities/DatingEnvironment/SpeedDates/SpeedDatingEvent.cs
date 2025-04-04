

using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.DatingEnvironment.SpeedDates
{
    public class SpeedDatingEvent : Record
    {
        public string Title { get; set; } = default!;
        public SpeedDatingCategory Category { get; set; } = SpeedDatingCategory.Heterosexual;
        public DateTime StartTime { get; set; }

        public DateTime? StartSessionTime { get; set; }
        public DateTime? EndSessionTime { get; set; }

        public string? ImageUrl { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public int MaxParticipants { get; set; }
        public int DurationPerPairingMinutes { get; set; }

        public DateStatus Status { get; set; } = DateStatus.Pending;

        // Track the creator
        public int? ContentCreationStatus { get; set; }
        public bool CreatorIsAdmin { get; set; }
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;

        public int LocationId { get; set; }
        public Location Location { get; set; } = default!;

        public List<SpeedDatingParticipant> Participants { get; set; } = new();
    }
}
