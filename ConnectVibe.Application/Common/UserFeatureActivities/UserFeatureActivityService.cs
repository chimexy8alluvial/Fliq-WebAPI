

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
            _logger.LogInfo($"[TrackUserFeatureActivity] Start - UserId: {userId}, Feature: {featureName}");

            var existingActivity = await _userFeatureActivityRepository.GetUserFeatureActivityAsync(userId, featureName);

            if (existingActivity != null)
            {
                existingActivity.LastActiveAt = DateTime.UtcNow;
                existingActivity.DateModified = DateTime.UtcNow;
                _logger.LogInfo($"[TrackUserFeatureActivity] Updating existing activity for UserId: {userId}, Feature: {featureName}");
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
                _logger.LogInfo($"[TrackUserFeatureActivity] Creating new activity record for UserId: {userId}, Feature: {featureName}");
            }

             await _userFeatureActivityRepository.AddAsync(existingActivity);
            _logger.LogInfo($"[TrackUserFeatureActivity] Successfully tracked feature activity for UserId: {userId}, Feature: {featureName}");
        }


    }
}
