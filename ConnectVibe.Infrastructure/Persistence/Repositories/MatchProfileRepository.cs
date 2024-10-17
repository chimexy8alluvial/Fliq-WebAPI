using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Entities.MatchedProfile;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class MatchProfileRepository : IMatchProfileRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public MatchProfileRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _dbContext = dbContext;
        }
        public void Add(MatchRequest matchProfile)
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

        public async Task<IEnumerable<MatchRequestDto>> GetMatchListById(int userId, int pageNumber, int pageSize)
        {
            //Initializing values for page number and size.
            pageNumber = 1;
            pageSize = 10;

            var filteredItems = await _dbContext.MatchRequests
                .Where(p => p.UserId == userId && p.matchRequestStatus == Domain.Enums.MatchRequestStatus.Pending)
                .Select(p => new MatchRequestDto
                {
                    MatchInitiatorUserId = p.MatchInitiatorUserId,
                    Name = p.Name,
                    PictureUrl = p.PictureUrl
                })
                .Skip((pageNumber - 1) * pageSize) // Skip items for previous pages
                .Take(pageSize) // Take only the pageSize number of items
                .ToListAsync();

            return filteredItems;
        }

        public MatchRequest? GetMatchProfileByUserId(int Id)
        {
            var matchProfile = _dbContext.MatchRequests.SingleOrDefault(p => p.UserId == Id);
            return matchProfile;
        }


    }
}
