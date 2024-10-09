using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        UserProfile? GetUserProfileByUserId(int id);

        IEnumerable<UserProfile> GetMatchedUserProfiles(int userId, List<ProfileType> userProfileTypes, bool? filterByDating, bool? filterByFriendship, int pageNumber,int pageSize);
    }
}