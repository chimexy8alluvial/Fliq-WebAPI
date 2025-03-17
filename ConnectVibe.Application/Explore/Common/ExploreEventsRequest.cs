using Fliq.Application.Common.Pagination;

namespace Fliq.Application.Explore.Common
{
    public record ExploreEventsRequest(
        int PageNumber = 1, int PageSize = 5): PaginationRequest(PageNumber, PageSize);

}
