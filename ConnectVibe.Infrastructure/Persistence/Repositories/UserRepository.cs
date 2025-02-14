using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities;
using Dapper;
using System.Data;
using Microsoft.EntityFrameworkCore;

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
            var user = _dbContext.Users.SingleOrDefault(p => p.Email == email);
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

        public User? GetUserById(int id)
        {
            var user = _dbContext.Users.SingleOrDefault(p => p.Id == id);
            return user;
        }
        //To be changed to stored procedure
        public User? GetUserByIdIncludingProfile(int id)
        {
            var user = _dbContext.Users.Include(p=>p.UserProfile).ThenInclude(p => p.Photos).SingleOrDefault(p => p.Id == id);
            return user;
        }

        //Count Queries
        public async Task<int> CountActiveUsers()
        {
            return await _dbContext.Users.CountAsync(u => u.IsActive); // Using IsActive flag
        }

        public async Task<int> CountInactiveUsers()
        {
            return await _dbContext.Users.CountAsync(u => !u.IsActive); // Using IsActive flag
        }

        public async Task<int> CountNewSignups(int days)
        {
            return await _dbContext.Users.CountAsync(u => u.DateCreated >= DateTime.UtcNow.AddDays(-days));
        }
    }
}
