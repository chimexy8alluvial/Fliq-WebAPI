using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.DatingEnvironment.BlindDates
{
    public class BlindDate : Record
    {
        public int CategoryId { get; set; }
        public BlindDateCategory BlindDateCategory { get; set; } = default!;

        public string Title { get; set; } = default!;
        public DateTime StartDateTime { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; } = default!;

        public bool IsOneOnOne { get; set; }
        public int? NumberOfParticipants { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsRecordingEnabled { get; set; } = false;
        public string? RecordingUrl { get; set; }

        public DateTime? SessionStartTime { get; set; }
        public DateTime? SessionEndTime { get; set; }
        public TimeSpan? Duration =>
            SessionStartTime.HasValue && SessionEndTime.HasValue
            ? SessionEndTime - SessionStartTime
            : null;

        public DateStatus Status { get; set; } = DateStatus.Pending;

        // Track the creator
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;

        public ICollection<BlindDateParticipant> Participants { get; set; } = new List<BlindDateParticipant>();

        // Ensure proper validation when saving/updating
        public void Validate()
        {
            if (IsOneOnOne && NumberOfParticipants is not (null or 1))
            {
                throw new ArgumentException("One-on-one blind dates must have exactly 1 participant.");
            }

            if (!IsOneOnOne && (NumberOfParticipants == null || NumberOfParticipants < 2))
            {
                throw new ArgumentException("Group blind dates must have at least 2 participants.");
            }
        }

    }
}
