using Dapper;
using Fliq.Application.Common.Interfaces.Helper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Event;
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
        private readonly ILoggerManager _logger;

        public ProfileRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, ICustomProfileMapper customProfileMapper, ILoggerManager logger)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _customProfileMapper = customProfileMapper;
            _logger = logger;
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

        public PaginationResponse<UserProfile> GetMatchedUserProfiles(
             int userId,
             List<ProfileType> userProfileTypes,
             bool? filterByDating,
             bool? filterByFriendship,
             bool? filterByEvent,
             PaginationRequest paginationRequest)
                {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = CreateDynamicParameters(userId, userProfileTypes, filterByDating, filterByFriendship,filterByEvent, paginationRequest);

                var sql = "sPGetMatchedUserProfiles";

                // Execute the query using Dapper
                var result = connection.Query<dynamic>(sql, param: parameters, commandType: CommandType.StoredProcedure).ToList();

                // Extract TotalCount (assume it's the same for all rows)
                int totalCount = result.Any() ? result.First().TotalCount : 0;

                // Map profiles (no grouping needed)
                var profiles = result.Select(row => _customProfileMapper.MapToUserProfile(new[] { row }.GroupBy(r => (int)r.Id).First()))
                                    .Where(profile => profile != null)
                                    .ToList();

                return new PaginationResponse<UserProfile>(
                    data: profiles,
                    totalCount: totalCount,
                    pageNumber: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize
                );
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
        private static DynamicParameters CreateDynamicParameters(int userId, List<ProfileType> userProfileTypes, bool? filterByDating, bool? filterByFriendship,bool? filterByEvent, PaginationRequest paginationRequest)
        {
            string serializedProfileTypes = JsonConvert.SerializeObject(userProfileTypes);

            var parameters = new DynamicParameters();

            parameters.Add("@userId", userId);

            // Convert ProfileType enum values to integers and join them as a comma-separated string
            parameters.Add("@profileTypes", serializedProfileTypes);  // Comma-separated list of integers

            parameters.Add("@filterByDating", filterByDating, DbType.Boolean);
            parameters.Add("@filterByFriendship", filterByFriendship, DbType.Boolean);
            parameters.Add("@filterByEvent", filterByEvent, DbType.Boolean);
            parameters.Add("@pageNumber", paginationRequest.PageNumber);
            parameters.Add("@pageSize", paginationRequest.PageSize);

            return parameters;
        }

        public async Task<string?> GetUserCountryAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var userProfile = await _dbContext.UserProfiles
                    .Include(p => p.Location)
                    .ThenInclude(l => l.LocationDetail)
                    .ThenInclude(ld => ld.Results)
                    .ThenInclude(r => r.AddressComponents)
                    .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

                if (userProfile?.Location?.LocationDetail?.Results?.Any() != true)
                {
                    _logger.LogWarn($"No profile or location found for user ID {userId}.");
                    return null;
                }

                var addressComponent = userProfile.Location.LocationDetail.Results
                    .SelectMany(r => r.AddressComponents)
                    .FirstOrDefault(ac => ac.Types.Contains("country"));

                var country = addressComponent?.ShortName; // e.g., "US", "NG"
                if (string.IsNullOrEmpty(country))
                {
                    _logger.LogWarn($"No country found in address components for user ID {userId}.");
                }
                else
                {
                    _logger.LogInfo($"Retrieved country {country} for user ID {userId}.");
                }

                return country;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving country for user ID {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<Currency> GetUserCurrencyAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                // Get user's country
                var userCountry = await GetUserCountryAsync(userId, cancellationToken);

                // Find currency for user's country
                Currency? currency = null;
                if (!string.IsNullOrEmpty(userCountry))
                {
                    currency = await _dbContext.Currencies
                        .FirstOrDefaultAsync(c => c.CountryCode == userCountry && !c.IsDeleted, cancellationToken);
                }

                // Fallback to default currency (USD) if no match
                if (currency == null)
                {
                    _logger.LogWarn($"No currency found for country {userCountry ?? "unknown"} for user ID {userId}. Using default (USD).");
                    currency = await _dbContext.Currencies
                        .FirstOrDefaultAsync(c => c.CurrencyCode == "USD" && !c.IsDeleted, cancellationToken);
                    if (currency == null)
                    {
                        _logger.LogError("Default currency (USD) not found.");
                        throw new InvalidOperationException("No currency available for ticket creation.");
                    }
                }

                _logger.LogInfo($"Selected currency {currency.CurrencyCode} (ID: {currency.Id}) for user ID {userId}.");
                return currency;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving currency for user ID {userId}: {ex.Message}");
                throw;
            }
        }
    }
}