using Fliq.Application.Explore.Queries;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Explore.Common.Services
{
    public interface IProfileMatchingService
    {
        Task<IEnumerable<UserProfile>> GetMatchedProfilesAsync(User user, ExploreQuery query);
    }
}