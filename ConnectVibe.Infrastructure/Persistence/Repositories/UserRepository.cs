using Dapper;
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.Profile.Common;
using Fliq.Application.Users.Common;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        public UserRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(User user)
        {
            if (user.Id > 0)
            {
                user.DateModified = DateTime.Now;
                _dbContext.Update(user);
            }
            else
            {
                _dbContext.Add(user);
            }
            _dbContext.SaveChanges();
        }
        public void Update(User user)
        {
            user.DateModified = DateTime.Now;

            _dbContext.Update(user);

            _dbContext.SaveChanges();
        }

        public User? GetUserByEmail(string email)
        {
            var user = _dbContext.Users.Include(u => u.Role).SingleOrDefault(p => p.Email == email);
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var users = connection.Query<User>("sp_GetAllUsers", commandType: CommandType.StoredProcedure);
                return users;
            }
        }

         public async Task<IEnumerable<GetUsersResult>> GetAllUsersForDashBoardAsync(GetUsersListRequest query)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = FilterListDynamicParams(query);

                return await connection.QueryAsync<GetUsersResult>("sp_GetAllUsersForDashBoard",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                       
            }
        }

        public async Task<List<User>> GetInactiveUsersAsync(DateTime thresholdDate)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_GetInactiveUsers";
                var parameter = new { ThresholdDate = thresholdDate };

                var users = await connection.QueryAsync<User>(sql, parameter, commandType: CommandType.StoredProcedure);
                return users.ToList();
            }
        }

        public async Task<IEnumerable<UsersTableListResult>> GetAllUsersByRoleIdAsync(int roleId, int pageNumber, int pageSize)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_GetUsersWithLatestSubscription";
                var parameter = new
                {
                    RoleId = roleId,
                    Offset = (pageNumber - 1) * pageSize,
                    Fetch = pageSize
                };

                var results = await connection.QueryAsync<dynamic>(sql, parameter, commandType: CommandType.StoredProcedure);

                return results.Select(x => new UsersTableListResult
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Subscription = x.SubscriptionType,
                    DateCreated = x.DateCreated,
                    LastActiveAt = x.LastActiveAt,
                });
            }
        }

        public User? GetUserById(int id)
        {
            var user = _dbContext.Users.Include(w => w.Wallet).SingleOrDefault(p => p.Id == id);
            return user;
        }

        //To be changed to stored procedure
        public User? GetUserByIdIncludingProfile(int id)
        {
            var user = _dbContext.Users.Include(p => p.UserProfile).ThenInclude(up => up.Photos).Include(p => p.UserProfile).
                ThenInclude(up => up.Gender).Include(p => p.UserProfile).ThenInclude(up => up.Religion).Include(p => p.UserProfile).ThenInclude(up => up.SexualOrientation).
                Include(p => p.UserProfile).ThenInclude(up => up.EducationStatus).Include(p => p.UserProfile).ThenInclude(up => up.Occupation).
                Include(p => p.UserProfile).ThenInclude(up => up.HaveKids).
                Include(p => p.UserProfile).ThenInclude(up => up.WantKids).SingleOrDefault(p => p.Id == id);
            return user;
        }


        private static DynamicParameters FilterListDynamicParams(GetUsersListRequest query)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@pageNumber", query.PaginationRequest!.PageNumber);
            parameters.Add("@pageSize", query.PaginationRequest.PageSize);
            parameters.Add("@hasSubscription", query.HasSubscription);
            parameters.Add("@activeSince", query.ActiveSince);
            parameters.Add("@roleName", query.RoleName);
            return parameters;
        }


        #region Count Queries

        public async Task<int> CountActiveUsers()
        {

            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountActiveUsers", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> CountInactiveUsers()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountInActiveUsers", commandType: CommandType.StoredProcedure); // Using IsActive flag
                return count;
            }
        }
        public async Task<int> CountAllUsers()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountAllUsers", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> CountNewSignups(int days)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_CountUsersCreatedInLastDays";
                var parameter = new { Days = days };
                var count = await connection.QueryFirstOrDefaultAsync<int>(sql, parameter, commandType: CommandType.StoredProcedure); // Using IsActive flag
                return count;
            }
        }

        public async Task<ErrorOr<ProfileDataTablesResponse>> GetAllProfileSetupData(CancellationToken cancellationToken)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var commandDefinition = new CommandDefinition(
             "sp_GetAllProfileData",
             commandType: CommandType.StoredProcedure,
             cancellationToken: cancellationToken);
                using var multi = await connection.QueryMultipleAsync(commandDefinition);
                var result = new ProfileDataTablesResponse
                {
                    Occupations = (await multi.ReadAsync<Occupation>()).ToList(),
                    Religions = (await multi.ReadAsync<Religion>()).ToList(),
                    Ethnicities = (await multi.ReadAsync<Ethnicity>()).ToList(),
                    EducationStatuses = (await multi.ReadAsync<EducationStatus>()).ToList(),
                    Genders = (await multi.ReadAsync<Gender>()).ToList(),
                    HaveKids = (await multi.ReadAsync<HaveKids>()).ToList(),
                    WantKids = (await multi.ReadAsync<WantKids>()).ToList(),
                    BusinessIdentificationDocuments = (await multi.ReadAsync<BusinessIdentificationDocument>()).ToList(),
                };

                if (!result.Occupations.Any() &&
                    !result.Religions.Any() &&
                    !result.Ethnicities.Any() &&
                    !result.EducationStatuses.Any() &&
                    !result.Genders.Any() &&
                    !result.HaveKids.Any() &&
                    !result.WantKids.Any() &&
                    !result.BusinessIdentificationDocuments.Any() 

                    )
                {
                    return Error.NotFound(description: "No profile setup data found");
                }

                return result;
            }
            catch (SqlException ex)
            {
                return Error.Failure(
                    code: "DatabaseError",
                    description: $"Failed to retrieve profile setup data: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Error.Unexpected(
                    description: $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<int> CountAllMaleUsers()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountAllMaleUsers", commandType: CommandType.StoredProcedure);
                return count;
            }
        }
        
        public async Task<int> CountAllFemaleUsers()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountAllFemaleUsers", commandType: CommandType.StoredProcedure);
                return count;
            }
        }
        
        public async Task<int> CountAllOtherUsers()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountAllOtherUsers", commandType: CommandType.StoredProcedure);
                return count;
            }
        }
        #endregion
    }
}
