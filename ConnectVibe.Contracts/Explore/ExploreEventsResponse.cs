namespace Fliq.Contracts.Explore
{
    public record ExploreEventsResponse
    {
        public IEnumerable<EventWithDisplayName> Items { get; init; } = Enumerable.Empty<EventWithDisplayName>();
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
        public bool HasNextPage { get; init; }
        public bool HasPreviousPage { get; init; }
    }
   
}