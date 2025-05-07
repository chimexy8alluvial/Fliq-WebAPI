namespace Fliq.Contracts.Explore
{
    public record EventReviewDto
    {
        public int UserId { get; init; }
        public int EventId { get; init; }
        public int Rating { get; init; }
        public string Comments { get; init; } = string.Empty;
    }

}
