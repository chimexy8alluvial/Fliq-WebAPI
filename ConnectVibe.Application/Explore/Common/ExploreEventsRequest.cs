using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Application.Explore.Common
{
    public record ExploreEventsRequest
    {
        public double? MaxDistanceKm { get; init; }
        public EventCategory? Category { get; init; }
        public EventType? EventType { get; init; }
        public int? CreatorId { get; init; }
        public EventStatus? EventStatus { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 5;
    }

}
