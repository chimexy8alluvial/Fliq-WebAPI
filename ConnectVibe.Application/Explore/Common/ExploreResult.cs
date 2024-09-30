using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Explore.Common
{
    public record ExploreResult(
       PaginationResponse<UserProfile> UserProfiles
        /*IEnumerable<Event>? Events = null*/);
    
}
