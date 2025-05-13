using Dapper;
using Fliq.Application.Common.Interfaces.Helper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICustomProfileMapper _customProfileMapper;

        public ProfileRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, ICustomProfileMapper customProfileMapper)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _customProfileMapper = customProfileMapper;
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

        public IEnumerable<UserProfile> GetMatchedUserProfiles(
        int userId,
        List<ProfileType> userProfileTypes,
        bool? filterByDating,
        bool? filterByFriendship,

        PaginationRequest paginationRequest)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = CreateDynamicParameters(userId, userProfileTypes, filterByDating, filterByFriendship, paginationRequest);

                var sql = "sPGetMatchedUserProfiles";

                // Execute the query using Dapper, returning a flat result.
                var result = connection.Query<dynamic>(sql, param: parameters, commandType: CommandType.StoredProcedure);

                // Group the result by UserProfileId to ensure that all rows belonging to the same UserProfile are processed together.
                var profiles = result.GroupBy(row => (int)row.Id)
                                     .Select(group => _customProfileMapper.MapToUserProfile(group)) // Pass each group to the mapper
                                     .ToList();

                return profiles;
            }
        }

        public UserProfile? GetProfileByUserId(int id)
        {
            var query = _dbContext.UserProfiles.AsQueryable();

            // Include all top-level navigation properties via reflection
            foreach (var property in _dbContext.Model.FindEntityType(typeof(UserProfile)).GetNavigations())
            {
                query = query.Include(property.Name);
            }

            // Explicitly include Location.LocationDetail
            query = query.Include(p => p.Location).ThenInclude(l => l.LocationDetail);

            var profile = query.SingleOrDefault(p => p.UserId == id);

            return profile;
        }
        //public UserProfile? GetProfileByUserId(int id)
        //{
        //    try
        //    {
        //        // First try without includes to isolate the issue
        //        var simpleProfile = _dbContext.UserProfiles
        //            .AsNoTracking()
        //            .FirstOrDefault(p => p.UserId == id);

        //        if (simpleProfile != null)
        //        {
        //            // If this works, the issue is in a navigation property
        //            return LoadWithIncludes(id);
        //        }
        //        return null;
        //    }
        //    catch (InvalidCastException ex)
        //    {
        //        Console.WriteLine($"Type cast error getting profile: {ex.Message}");
        //        throw;
        //    }
        //}

        //private UserProfile? LoadWithIncludes(int id)
        //{
        //    var query = _dbContext.UserProfiles.AsQueryable();

        //    // Add includes one by one to find the problematic one
        //    query = query.Include(p => p.User);
        //    // Test after each include
        //    query = query.Include(p => p.Photos);
        //    query = query.Include(p => p.Gender);
        //    query = query.Include(p => p.SexualOrientation);
        //    query = query.Include(p => p.EducationStatus);
        //    query = query.Include(p => p.Ethnicity);
        //    query = query.Include(p => p.Location);
        //    query = query.Include(p => p.Occupation);
        //    query = query.Include(p => p.Religion);
        //    query = query.Include(p => p.PromptResponses);
        //    // Continue with other navigation properties...

        //    return query.FirstOrDefault(p => p.UserId == id);
        //}
        private static DynamicParameters CreateDynamicParameters(int userId, List<ProfileType> userProfileTypes, bool? filterByDating, bool? filterByFriendship, PaginationRequest paginationRequest)
        {
            string serializedProfileTypes = JsonConvert.SerializeObject(userProfileTypes);

            var parameters = new DynamicParameters();

            parameters.Add("@userId", userId);

            // Convert ProfileType enum values to integers and join them as a comma-separated string
            parameters.Add("@profileTypes", serializedProfileTypes);  // Comma-separated list of integers

            parameters.Add("@filterByDating", filterByDating, DbType.Boolean);
            parameters.Add("@filterByFriendship", filterByFriendship, DbType.Boolean);
            parameters.Add("@pageNumber", paginationRequest.PageNumber);
            parameters.Add("@pageSize", paginationRequest.PageSize);

            return parameters;
        }
    }
}