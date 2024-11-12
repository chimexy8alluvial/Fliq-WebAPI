using Fliq.Application.MatchedProfile.Commands.ApprovedMatchedList;
using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Contracts.MatchedProfile;
namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IMatchProfileRepository
    {
        void Add(Domain.Entities.MatchedProfile.MatchRequest matchRequest);
        void Update(Domain.Entities.MatchedProfile.MatchRequest matchRequest);
        Domain.Entities.MatchedProfile.MatchRequest? GetMatchProfileByUserId(int id);
        Domain.Entities.MatchedProfile.MatchRequest? GetMatchProfileById(int id);
        Task<IEnumerable<MatchRequestDto>> GetMatchListById(GetMatchRequestListCommand query);
        Task<IEnumerable<MatchRequestDto>> GetApproveMatchListById(GetApprovedMatchListCommand query);
    }
}
