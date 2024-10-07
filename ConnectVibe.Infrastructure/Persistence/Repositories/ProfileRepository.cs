using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public ProfileRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(UserProfile userProfile)
        {
            if (userProfile.Id > 0)
            {
                _dbContext.Update(userProfile);
            }
            else
            {
                _dbContext.Add(userProfile);
            }
            _dbContext.SaveChanges();
        }

        public void Update(UserProfile profile)
        {
            _dbContext.Update(profile);

            _dbContext.SaveChanges();
        }

        public UserProfile? GetUserProfileByUserId(int id)
        {
            var profile = _dbContext.UserProfiles.SingleOrDefault(p => p.UserId == id);
            return profile;
        }

        public UserProfile? GetProfileByUserId(int id)
        {
            var query = _dbContext.UserProfiles.AsQueryable();

            // Use reflection to include all navigation properties
            foreach (var property in _dbContext.Model.FindEntityType(typeof(UserProfile)).GetNavigations())
            {
                query = query.Include(property.Name);
            }

            var profile = query.SingleOrDefault(p => p.UserId == id);
            return profile;
        }
    }
}