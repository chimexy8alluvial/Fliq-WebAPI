using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Contracts.Polls;
using Fliq.Domain.Entities.VotingPoll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class PollRepository : IPollRepository
    {
        public void CreateVote(VotePoll votePoll)
        {
            throw new NotImplementedException();
        }

        public VotePoll GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VotingListDto>> GetVotingList(int userId)
        {
            throw new NotImplementedException();
        }

        public void Vote(VotePoll votePoll)
        {
            throw new NotImplementedException();
        }
    }
}
