using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Explore.Common
{
    public record ExploreEventsResult(
       PaginationResponse<Events> Events
   );
}
