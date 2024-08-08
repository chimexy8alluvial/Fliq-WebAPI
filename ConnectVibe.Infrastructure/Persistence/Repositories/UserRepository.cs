using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        public UserRepository(ConnectVibeDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
