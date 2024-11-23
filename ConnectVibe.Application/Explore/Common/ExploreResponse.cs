using Fliq.Application.Common.Pagination;
using Fliq.Contracts.Profile;

namespace Fliq.Application.Explore.Common
{
    public record ExploreResponse(PaginationResponse<ProfileResponse> UserProfiles);
    //int TotalCount,
    //int PageNumber,
    //int PageSize);
}
