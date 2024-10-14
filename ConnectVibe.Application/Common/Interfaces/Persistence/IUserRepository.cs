using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository
    {
        void Add(User user);
        void Update(User user);
        User? GetUserByEmail(string email);
        IEnumerable<User> GetAllUsers();
        User? GetUserById(int Id);
    }
}
