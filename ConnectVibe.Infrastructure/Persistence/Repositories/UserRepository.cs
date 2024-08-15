using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Domain.Entities;
using Dapper;
using System.Data;

namespace ConnectVibe.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        public UserRepository(ConnectVibeDbContext dbContext, IDbConnectionFactory connectionFactory)
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
        public User? GetUserById(int id)
        {
            var user = _dbContext.Users.SingleOrDefault(p => p.Id == id);
            return user;
        }

}
}
