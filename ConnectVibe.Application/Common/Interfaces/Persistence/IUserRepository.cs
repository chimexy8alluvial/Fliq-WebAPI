using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository
    {
        void Add(User user);
        void Update(User user);
        User? GetUserByEmail(string email);
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetAllUsersForDashBoard(int pageNumber, int pageSize,
            bool? hasSubscription = null,
            DateTime? activeSince = null,
            string roleName = null);
        User? GetUserById(int Id);
        User? GetUserByIdIncludingProfile(int Id);
        Task<List<User>> GetInactiveUsersAsync(DateTime thresholdDate);

        //Count Queries
        Task<int> CountActiveUsers();
        Task<int> CountInactiveUsers();
        Task<int> CountAllUsers();
        Task<int> CountNewSignups(int days);
        Task<int> CountAllMaleUsers();
        Task<int> CountAllFemaleUsers();  
        Task<int> CountAllOtherUsers();
    }
}
