using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        UserProfile? GetUserProfileByUserId(int id);

        Task<IEnumerable<UserProfile>> GetProfilesAsync(int userId, int pageNumber, int pageSize, bool? filterByDating = null, bool? filterByFriendship = null);
    }
}