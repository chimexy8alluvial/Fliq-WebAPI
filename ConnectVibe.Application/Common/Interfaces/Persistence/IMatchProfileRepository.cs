using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Entities.MatchedProfile;
namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IMatchProfileRepository
    {
        void Add(MatchRequest matchRequest);
        MatchRequest? GetMatchProfileByUserId(int id);
        Task <IEnumerable<MatchRequestDto>> GetMatchListById(int userId, int pageNumber, int pageSize);
    }
}
