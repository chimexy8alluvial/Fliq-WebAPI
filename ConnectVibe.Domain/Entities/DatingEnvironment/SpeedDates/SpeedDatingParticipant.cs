 namespace Fliq.Domain.Entities.DatingEnvironment.SpeedDates
{
    public class SpeedDatingParticipant : Record
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public bool HasCompletedAllRounds { get; set; } = false;

        public bool IsCreator { get; set; } = false;

        public int SpeedDatingEventId { get; set; }
        public SpeedDatingEvent SpeedDatingEvent { get; set; } = default!;
      
    }
}