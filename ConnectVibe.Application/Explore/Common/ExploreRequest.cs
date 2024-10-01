using Fliq.Application.Common.Pagination;

namespace Fliq.Application.Explore.Common
{
    public record ExploreRequest(bool? FilterByEvent,
        bool? FilterByDating,
        bool? FilterByFriendship, int PageNumber = 1, int PageSize = 5) : PaginationRequest(PageNumber, PageSize);
}
