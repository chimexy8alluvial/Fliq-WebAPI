using Fliq.Application.Common.Pagination;

namespace Fliq.Application.MatchedProfile.Common
{
    public record MatchListRequest
    (
        int PageNumber = 1, int PageSize = 10) : PaginationRequest(PageNumber, PageSize
    );
}
