using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Infrastructure.Persistence;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.MatchedProfile;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class MatchProfileRepository : IMatchProfileRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public MatchProfileRepository(ConnectVibeDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _dbContext = dbContext;
        }
        public void Add(MatchProfile matchProfile)
        {
            if (matchProfile.Id > 0)
            {
                _dbContext.Update(matchProfile);
            }
            else
            {
                _dbContext.Add(matchProfile);
            }
            _dbContext.SaveChanges();
        }

        public async Task<IEnumerable<MatchProfile>> GetMatchListById(int userId, int pageNumber, int pageSize)
        {
            var filteredItems = _dbContext.MatchProfiles.Where(p => p.UserId == userId).Select(p => new
            {
                p.Age,
                p.UserName,
                p.RequestTime,
                p.Images
            })
            .ToList();

            return (IEnumerable<MatchProfile>)filteredItems;
        }

        public MatchProfile? GetMatchProfileByUserId(int Id)
        {
            var matchProfile = _dbContext.MatchProfiles.SingleOrDefault(p => p.UserId == Id);
            return matchProfile;
        }


    }
}
