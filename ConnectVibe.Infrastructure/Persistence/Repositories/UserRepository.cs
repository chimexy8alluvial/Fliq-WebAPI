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
    }
}
