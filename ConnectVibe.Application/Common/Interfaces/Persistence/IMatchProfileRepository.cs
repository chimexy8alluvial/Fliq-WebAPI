using Fliq.Application.Common.Pagination;
using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Entities.Profile;
namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IMatchProfileRepository
    {
        void Add(Domain.Entities.MatchedProfile.MatchRequest matchRequest);
        void Update(Domain.Entities.MatchedProfile.MatchRequest matchRequest);
        Domain.Entities.MatchedProfile.MatchRequest? GetMatchProfileByUserId(int id);
        Domain.Entities.MatchedProfile.MatchRequest? GetMatchProfileById(int id);
        Task <IEnumerable<MatchRequestDto>> GetMatchListById(int userId, MatchListPagination matchListPagination);
    }
}
