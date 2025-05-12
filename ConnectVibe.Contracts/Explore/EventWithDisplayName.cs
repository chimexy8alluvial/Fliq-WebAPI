using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Explore
{
    public class EventWithDisplayName
    {
        public int Id { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;
        public string? EventType { get; set; }
        public string? EventCategory { get; set; }
        public string? Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; } = "Unknown";
        public LocationDto Location { get; set; } = default!;
        public EventCriteriaDto? EventCriteria { get; set; }
        public List<EventReviewDto> Reviews { get; set; } = new List<EventReviewDto>();
    }
}
