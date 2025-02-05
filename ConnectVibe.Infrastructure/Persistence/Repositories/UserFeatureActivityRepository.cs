using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.UserFeatureActivities;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class UserFeatureActivityRepository : IUserFeatureActivityRepository
    {
        private readonly FliqDbContext _dbContext;

        public UserFeatureActivityRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(UserFeatureActivity userActivity)
        {
            if (userActivity.Id > 0)
            {
                _dbContext.Update(userActivity);
            }
            else
            {
                _dbContext.Add(userActivity);
            }
           await _dbContext.SaveChangesAsync();
        }

        public async Task<UserFeatureActivity?> GetUserFeatureActivity(int userId, string feature)
        {
            return await _dbContext.UserFeatureActivities
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Feature == feature);
        }
    }
}
