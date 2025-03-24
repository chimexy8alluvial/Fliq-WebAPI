using Fliq.Application.Users.Common;
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
        User? GetUserByIdIncludingProfile(int Id);
        Task<List<User>> GetInactiveUsersAsync(DateTime thresholdDate);
        Task<IEnumerable<UsersTableListResult>> GetAllUsersByRoleIdAsync(int roleId);

        //Count Queries
        Task<int> CountActiveUsers();
        Task<int> CountInactiveUsers();
        Task<int> CountAllUsers();
        Task<int> CountNewSignups(int days);
    }
}
