using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        UserProfile? GetUserProfileByUserId(int id);

        Task<List<UserProfile>> GetProfilesByStoredProcedureAsync(int userId, List<ProfileType> userProfileTypes, bool? filterByDating, bool? filterByFriendship);
    }
}