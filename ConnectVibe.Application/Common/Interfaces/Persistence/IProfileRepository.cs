using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        UserProfile? GetUserProfileByUserId(int id);

        Task<IEnumerable<UserProfile>> GetProfilesAsync(int userId, bool? filterByDating = null, bool? filterByFriendship = null);
    }
}