using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Microsoft.Data.SqlClient;
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

        public UserProfile? GetUserProfileByUserId(int id)
        {
            var profile = _dbContext.UserProfiles.SingleOrDefault(p => p.UserId == id);
            return profile;
        }

        public async Task<List<UserProfile>> GetProfilesByStoredProcedureAsync(int userId, List<ProfileType> userProfileTypes, bool? filterByDating, bool? filterByFriendship)
        {
            var profileTypesString = string.Join(",", userProfileTypes.Select(pt => pt.ToString()));

            var userIdParam = new SqlParameter("@UserId", userId);
            var profileTypesParam = new SqlParameter("@ProfileTypes", profileTypesString);
            var filterByDatingParam = new SqlParameter("@FilterByDating", filterByDating ?? (object)DBNull.Value);
            var filterByFriendshipParam = new SqlParameter("@FilterByFriendship", filterByFriendship ?? (object)DBNull.Value);

            return await _dbContext.UserProfiles
                .FromSqlRaw("EXEC GetUserProfiles @UserId, @ProfileTypes, @FilterByDating, @FilterByFriendship",
                            userIdParam, profileTypesParam, filterByDatingParam, filterByFriendshipParam)
                .ToListAsync();
        }

    }
}