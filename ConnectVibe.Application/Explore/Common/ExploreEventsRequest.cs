using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Application.Explore.Common
{
    public record ExploreEventsRequest
    {
        public double? MaxDistanceKm { get; init; }
        public EventCategory? Category { get; init; }
        public EventType? EventType { get; init; }
        public string? CreatedBy { get; init; }
        public string? EventTitle { get; init; }
        public EventStatus? EventStatus { get; init; }
        public bool? IncludeReviews { get; init; }
        public int? MinRating { get; init; }
        public PaginationRequest PaginationRequest { get; init; } = default!;
    }

}
