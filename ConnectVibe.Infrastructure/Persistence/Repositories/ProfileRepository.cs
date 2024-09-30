using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
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

        public async Task<IEnumerable<UserProfile>> GetProfilesAsync(int userId, int pageNumber, int pageSize, bool? filterByDating = null, bool? filterByFriendship = null)
        {
            // Retrieve the logged-in user's own profile types
            var userProfileTypes = await _dbContext.UserProfiles
                .Where(up => up.UserId == userId)
                .Select(up => up.ProfileTypes)
                .SingleOrDefaultAsync();

            if(userProfileTypes == null) 
                return []; // Return empty if no profileType is found for logged in user

            var query = _dbContext.UserProfiles.AsQueryable();

            //Ensure user can only explore profiles that match their own profile types
            query = query.Where(up => up.ProfileTypes.Any(profileType => userProfileTypes.Contains(profileType)));

            //Apply additional filters (if passed)
            if (filterByDating != null && true)
            {
                query = query.Where(up => up.ProfileTypes.Contains(ProfileType.Dating)
                    && up.UserId != userId)
                    .Include(up => up.User);
            }

            if (filterByFriendship == true)
            {
                query = query.Where(up => up.ProfileTypes.Contains(ProfileType.Friendship)
                    && up.UserId != userId)
                    .Include(up => up.User);
            }

            //Apply pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var queriedProfiles = await query.ToListAsync();

            return queriedProfiles;
        }

        public UserProfile? GetUserProfileByUserId(int id)
        {
            var profile = _dbContext.UserProfiles.SingleOrDefault(p => p.UserId == id);
            return profile;
        }
    }
}