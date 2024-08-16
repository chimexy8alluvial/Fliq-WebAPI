using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Domain.Entities.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Infrastructure.Persistence.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public ProfileRepository(ConnectVibeDbContext dbContext, IDbConnectionFactory connectionFactory)
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

        public UserProfile? GetUserProfileByUserId(int id)
        {
            var profile = _dbContext.UserProfiles.SingleOrDefault(p => p.UserId == id);
            return profile;
        }
    }
}