using ErrorOr;
using Fliq.Application.Profile.Common;
using Fliq.Application.DashBoard.Common;
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
       Task<IEnumerable<GetUsersResult>> GetAllUsersForDashBoardAsync(GetUsersListRequest query);
        User? GetUserById(int Id);
        User? GetUserByIdIncludingProfile(int Id);
        Task<List<User>> GetInactiveUsersAsync(DateTime thresholdDate);
        Task<List<User>> GetActiveUsersInBatchAsync(DateTime thresholdDate, int batchSize);
        Task<ErrorOr<ProfileDataTablesResponse>> GetAllProfileSetupData(CancellationToken cancellationToken);
        Task<IEnumerable<UsersTableListResult>> GetAllUsersByRoleIdAsync(int roleId, int pageNumber, int pageSize);

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
