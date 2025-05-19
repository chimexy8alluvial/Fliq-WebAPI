using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Common.Interfaces.Helper
{
    public interface ICustomProfileMapper
    {
        UserProfile MapToUserProfile(dynamic row);
    }
}
