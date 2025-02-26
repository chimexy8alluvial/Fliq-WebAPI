
using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.DatingEnvironment
{
    public class BlindDate : Record
    {
        public int CategoryId { get; set; }
        public BlindDateCategory BlindDateCategory { get; set; } = default!;

        public string Title { get; set; } = default!;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Location { get; set; } = default!;

        public bool IsOneOnOne { get; set; }
        public int? NumberOfParticipants { get; set; }

        public string? ImageUrl { get; set; } 

        public bool IsRecordingEnabled { get; set; } = false;
        public string? RecordingUrl { get; set; }

        public DateTime? SessionStartTime { get; set; }
        public DateTime? SessionEndTime { get; set; }
        public TimeSpan? Duration =>
            (SessionStartTime.HasValue && SessionEndTime.HasValue)
            ? SessionEndTime - SessionStartTime
            : null; 

        public BlindDateStatus Status { get; set; } = BlindDateStatus.Pending; 


        public ICollection<BlindDateParticipant> Participants { get; set; } = new List<BlindDateParticipant>();

        // Ensure proper validation when saving/updating
        public void Validate()
        {
            if (!IsOneOnOne && (NumberOfParticipants == null || NumberOfParticipants <= 1))
            {
                throw new ArgumentException("Number of participants must be greater than 1 for group blind dates.");
            }
        }
    }
}
