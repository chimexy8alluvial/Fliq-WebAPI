namespace Fliq.Domain.Entities.DatingEnvironment.BlindDates
{
    public class BlindDateParticipant : Record
    {
        public int BlindDateId { get; set; }
        public BlindDate BlindDate { get; set; } = default!;

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public bool IsCreator { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
