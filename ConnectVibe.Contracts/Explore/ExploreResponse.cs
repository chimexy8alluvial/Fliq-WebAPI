using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Explore
{ 
      

    public record ExploreResponse
    {
        public IEnumerable<ExploreProfileResponse> Data { get; init; } = Enumerable.Empty<ExploreProfileResponse>();
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
        public bool HasNextPage { get; init; }
        public bool HasPreviousPage { get; init; }
    }
}
