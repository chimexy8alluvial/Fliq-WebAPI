using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Queries;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Explore.Common.Services
{
    public interface IProfileMatchingService
    {
        Task<PaginationResponse<UserProfile>> GetMatchedProfilesAsync(User user, ExploreQuery query);
    }
}