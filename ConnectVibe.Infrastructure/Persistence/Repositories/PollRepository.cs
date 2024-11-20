using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Contracts.Polls;
using Fliq.Domain.Entities.VotingPoll;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class PollRepository : IPollRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public PollRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _dbContext = dbContext;
        }

        public void CreateVote(VotePoll votePoll)
        {
            if (votePoll.Id > 0)
            {
                _dbContext.Update(votePoll);
            }
            else
            {
                _dbContext.Add(votePoll);
            }
            _dbContext.SaveChanges();
        }

        public VotePoll? GetById(int id)
        {
            var user = _dbContext.VotePolls.SingleOrDefault(p => p.UserId == id);
            return user;
        }

        public async Task<IEnumerable<VotingListDto>> GetVotingList(int pollId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = DynamicParams(pollId);
                var result = connection.Query<dynamic>("spGetVotingPoll", param: parameters, commandType: CommandType.StoredProcedure);
                var filteredResult = result.Select(z => new VotingListDto
                {   EventId = z.EventId,
                    Count = z.Count,
                    multipleOptionSelect = z.multipleOptionSelect,
                    Options = z.options,
                    Picture = z.picture,
                    UserId = z.userId,
                    Question = z.question
                });
                return filteredResult;
            }
        }

        public void Vote(VotePoll votePoll)
        {
            if (votePoll.Id > 0)
            {
                _dbContext.Update(votePoll);
            }
            else
            {
                _dbContext.Add(votePoll);
            }
            _dbContext.SaveChanges();
        }

        private static DynamicParameters DynamicParams(int pollId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userId", pollId);
            return parameters;
        }
    }
}
