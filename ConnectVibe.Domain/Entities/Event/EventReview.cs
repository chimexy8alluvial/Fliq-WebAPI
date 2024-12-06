namespace Fliq.Domain.Entities.Event
{
    public class EventReview : Record
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public Events Event { get; set; } = default!;
        public int Rating { get; set; } // Rating out of 5
        public string Comments { get; set; } = default!;
    }
}