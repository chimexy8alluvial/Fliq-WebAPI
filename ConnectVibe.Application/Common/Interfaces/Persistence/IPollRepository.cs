using Fliq.Contracts.Polls;
using Fliq.Domain.Entities.VotingPoll;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPollRepository
    {
        void CreateVote(VotePoll votePoll);
        void Vote(VotePoll votePoll);
        VotePoll GetById(int id);
        Task<IEnumerable<VotingListDto>> GetVotingList(int userId);
    }
}
