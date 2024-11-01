using Fliq.Application.Common.Pagination;
using Fliq.Contracts.Profile;


namespace Fliq.Contracts.Explore
{
    public record ExploreResponse(PaginationResponse<ProfileResponse> UserProfiles);
        //int TotalCount,
        //int PageNumber,
        //int PageSize);
}
