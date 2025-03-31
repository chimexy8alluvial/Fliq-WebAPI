using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities;
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

         public IEnumerable<User> GetAllUsersForDashBoard(GetUsersListRequest query)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = FilterListDynamicParams(query);              

                var results = connection.Query<dynamic>("sp_GetAllUsersForDashBoard",
                    parameters,
                    commandType: CommandType.StoredProcedure);
               
                return   results.Select(r => new User
                {
                    DisplayName = r.DisplayName,
                    Email = r.Email,
                    DateCreated = r.DateJoined,
                    LastActiveAt = r.LastOnline,
                    Subscriptions = r.SubscriptionType != "None"
                        ? new List<Subscription> { new Subscription { ProductId = r.SubscriptionType } }
                        : null
                });
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

        public User? GetUserById(int id)
        {
            var user = _dbContext.Users.SingleOrDefault(p => p.Id == id);
            return user;
        }

        //To be changed to stored procedure
        public User? GetUserByIdIncludingProfile(int id)
        {
            var user = _dbContext.Users.Include(p=>p.UserProfile).ThenInclude(p => p!.Photos).SingleOrDefault(p => p.Id == id);
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
