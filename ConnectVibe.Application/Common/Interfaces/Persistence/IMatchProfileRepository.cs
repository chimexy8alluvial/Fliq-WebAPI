using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Entities.Profile;
namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IMatchProfileRepository
    {
        void Add(MatchRequest matchRequest);
        void Update(MatchRequest matchRequest);
        MatchRequest? GetMatchProfileByUserId(int id);
        MatchRequest? GetMatchProfileById(int id);
        Task <IEnumerable<MatchRequestDto>> GetMatchListById(int userId, int pageNumber, int pageSize);
    }
}
