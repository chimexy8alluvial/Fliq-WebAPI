

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.UserFeatureActivities;
using Fliq.Domain.Entities.UserFeatureActivities;

namespace Fliq.Application.Common.UserFeatureActivities
{
    public class UserFeatureActivityService : IUserFeatureActivityService
    {
        private readonly IUserFeatureActivityRepository _userFeatureActivityRepository;
        private readonly ILoggerManager _logger;

        public UserFeatureActivityService(IUserFeatureActivityRepository userFeatureActivityRepository, ILoggerManager logger)
        {
            _userFeatureActivityRepository = userFeatureActivityRepository;
            _logger = logger;
        }

        public async Task TrackUserFeatureActivity(int userId, string featureName)
        {
            var existingActivity = await _userFeatureActivityRepository.GetUserFeatureActivity(userId, featureName);

            if (existingActivity != null)
            {
                existingActivity.LastActiveAt = DateTime.UtcNow;
                existingActivity.DateModified = DateTime.UtcNow;
            }
            else
            {
                existingActivity = new UserFeatureActivity
                {
                    UserId = userId,
                    Feature = featureName,
                    LastActiveAt = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow
                };
            }

             await _userFeatureActivityRepository.Add(existingActivity);
        }
    }
}
