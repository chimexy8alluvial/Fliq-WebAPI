

namespace Fliq.Application.Common.Interfaces.UserFeatureActivities
{
    public interface IUserFeatureActivityService
    {
        Task TrackUserFeatureActivity(int userId, string featureName);
    }
}
