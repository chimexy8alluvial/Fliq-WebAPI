using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Explore
{
    public record ExploreEventsResponse
    {
        public IEnumerable<EventDto> Items { get; init; } = Enumerable.Empty<EventDto>();
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
        public bool HasNextPage { get; init; }
        public bool HasPreviousPage { get; init; }
    }

   

   
}