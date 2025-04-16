using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Explore
{
    public record EventDto
    {
        public int Id { get; init; }
        public string EventTitle { get; init; } = string.Empty;
        public string EventDescription { get; init; } = string.Empty;
        public string EventType { get; init; } = string.Empty;
        public string EventCategory { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public LocationDto Location { get; init; } = default!;
        public int UserId { get; init; }
    }
}
