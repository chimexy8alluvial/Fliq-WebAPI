using Fliq.Application.Common.Pagination;
using Fliq.Contracts.Explore;

namespace Fliq.Application.Explore.Common
{
    public record ExploreEventsResult(PaginationResponse<EventWithDisplayName> Events);

   
}
