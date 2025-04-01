using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities;
using Dapper;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Fliq.Application.Users.Common;

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
                var parameter = new { RoleId = roleId,
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
            var user = _dbContext.Users.SingleOrDefault(p => p.Id == id);
            return user;
        }

        //To be changed to stored procedure
        public User? GetUserByIdIncludingProfile(int id)
        {
            var user = _dbContext.Users.Include(p=>p.UserProfile).ThenInclude(p => p!.Photos).SingleOrDefault(p => p.Id == id);
            return user;
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

        #endregion
    }
}
