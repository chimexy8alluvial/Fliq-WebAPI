using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.DatingEnvironment
{
    public class DatingListItems
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DatingType? Type { get; set; }
        public string? CreatedBy { get; set; }
        public string? SubscriptionType { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
