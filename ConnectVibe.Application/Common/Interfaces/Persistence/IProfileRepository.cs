using ConnectVibe.Domain.Entities.Profile;

namespace ConnectVibe.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        UserProfile? GetUserProfileByUserId(int id);
    }
}