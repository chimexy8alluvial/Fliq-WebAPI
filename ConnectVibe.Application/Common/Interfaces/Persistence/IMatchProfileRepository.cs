using Fliq.Application.MatchedProfile.Common;
using Fliq.Contracts.MatchedProfile;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IMatchProfileRepository
    {
        void Add(Domain.Entities.MatchedProfile.MatchRequest matchRequest);

        void Update(Domain.Entities.MatchedProfile.MatchRequest matchRequest);

        Domain.Entities.MatchedProfile.MatchRequest? GetMatchProfileByUserId(int id);

        Domain.Entities.MatchedProfile.MatchRequest? GetMatchProfileById(int id);

        Task<IEnumerable<MatchRequestDto>> GetMatchListById(GetMatchListRequest query);
    }
}