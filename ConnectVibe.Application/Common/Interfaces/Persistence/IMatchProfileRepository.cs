using Fliq.Domain.Entities.MatchedProfile;
namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IMatchProfileRepository
    {
        void Add(MatchProfile matchProfile);
        MatchProfile? GetMatchProfileByUserId(int id);
        Task <IEnumerable<MatchProfile>> GetMatchListById(int userId, int pageNumber, int pageSize);
    }
}
