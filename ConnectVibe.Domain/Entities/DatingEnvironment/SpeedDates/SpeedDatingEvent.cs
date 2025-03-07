

using Fliq.Domain.Entities.Profile;

namespace Fliq.Domain.Entities.DatingEnvironment.SpeedDates
{
    public class SpeedDatingEvent : Record
    {
        public string Title { get; set; } = default!;
        public string Category { get; set; } = default!;
        public DateTime StartTime { get; set; }

        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public int MaxParticipants { get; set; }
        public int DurationPerPairingMinutes { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; } = default!;

        public List<SpeedDatingParticipant> Participants { get; set; } = new();
    }
}
