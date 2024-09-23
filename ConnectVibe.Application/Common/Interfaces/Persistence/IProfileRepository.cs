using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        UserProfile? GetUserProfileByUserId(int id);
    }
}