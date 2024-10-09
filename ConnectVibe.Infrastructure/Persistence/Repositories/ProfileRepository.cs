using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        public IEnumerable<UserProfile> GetMatchedUserProfiles(
            int userId,
            List<ProfileType> userProfileTypes,
            bool? filterByDating,
            bool? filterByFriendship,
            int pageNumber,
            int pageSize)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                // Convert profile types to a comma-separated string
                var profileTypesString = string.Join(",", userProfileTypes.Select(pt => pt.ToString()));

                // Define stored procedure parameters
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId);
                parameters.Add("@profileTypes", profileTypesString);
                parameters.Add("@filterByDating", filterByDating, DbType.Boolean);
                parameters.Add("@filterByFriendship", filterByFriendship, DbType.Boolean);
                parameters.Add("@pageNumber", pageNumber);
                parameters.Add("@pageSize", pageSize);

                // Call the stored procedure
                var profiles = connection.Query<UserProfile>(
                    "sPGetMatchedUserProfiles",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return profiles;
            }
        }


    }
}