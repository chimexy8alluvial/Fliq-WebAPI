using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        void Update(UserProfile profile);

        UserProfile? GetUserProfileByUserId(int id);

        UserProfile? GetProfileByUserId(int id);
    }
}