using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Recommendations.Queries;
using Fliq.Domain.Entities.Recommendations;
using Fliq.Domain.Enums.Recommendations;
using MediatR;
using Quartz;

namespace Fliq.Application.SchedulingServices.QuartzJobs
{
    public class PrecomputeRecommendationsJob : IJob
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ILoggerManager _logger;

        public PrecomputeRecommendationsJob(
            IMediator mediator,
            IUserRepository userRepository,
            IRecommendationRepository recommendationRepository,
            ILoggerManager logger)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _recommendationRepository = recommendationRepository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Starting recommendation pre-computation job");

            try
            {
                var jobDataMap = context.JobDetail.JobDataMap;
                var batchSize = jobDataMap.GetIntValue("BatchSize") > 0 ? jobDataMap.GetIntValue("BatchSize") : 50;
                var maxActiveUserDays = jobDataMap.GetIntValue("MaxActiveUserDays") > 0 ? jobDataMap.GetIntValue("MaxActiveUserDays") : 30;

                await PrecomputeRecommendationsForActiveUsers(batchSize, maxActiveUserDays);

                _logger.LogInfo("Recommendation pre-computation job completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in recommendation pre-computation job: {ex.Message}");

                // Create a JobExecutionException to indicate the job failed
                var jobException = new JobExecutionException(ex)
                {
                    RefireImmediately = false // Don't retry immediately
                };
                throw jobException;
            }
        }

        private async Task PrecomputeRecommendationsForActiveUsers(int batchSize, int maxActiveUserDays)
        {
            _logger.LogInfo($"Getting active users (last active within {maxActiveUserDays} days)");

            // Get active users in batches to avoid memory issues
            var activeUsers = await _userRepository.GetActiveUsersInBatchAsync(DateTime.UtcNow.AddDays(-maxActiveUserDays), batchSize);
            var totalUsers = activeUsers.Count();

            _logger.LogInfo($"Found {totalUsers} active users to process");

            var tasks = activeUsers.Select(user => PrecomputeRecommendationsForUser(user.Id));
            await Task.WhenAll(tasks);

            // Add a small delay between batches to reduce system load
            await Task.Delay(1000);
            
            _logger.LogInfo($"Completed recommendation pre-computation for {totalUsers} active users");
        }

        private async Task PrecomputeRecommendationsForUser(int userId)
        {
            try
            {
                _logger.LogInfo($"Pre-computing recommendations for user {userId}");

                // Check if user already has recent cached recommendations for each type
                var cacheValidTime = DateTime.UtcNow.AddHours(-6); // Cache valid for 6 hours

                var hasValidEventCache = await _recommendationRepository.HasValidCachedRecommendationsAsync(
                    userId, RecommendationType.Event, cacheValidTime);
                var hasValidBlindDateCache = await _recommendationRepository.HasValidCachedRecommendationsAsync(
                    userId, RecommendationType.BlindDate, cacheValidTime);
                var hasValidSpeedDateCache = await _recommendationRepository.HasValidCachedRecommendationsAsync(
                    userId, RecommendationType.SpeedDate, cacheValidTime);
                var hasValidUserCache = await _recommendationRepository.HasValidCachedRecommendationsAsync(
                    userId, RecommendationType.User, cacheValidTime);

                // If all caches are valid, skip this user
                if (hasValidEventCache && hasValidBlindDateCache && hasValidSpeedDateCache && hasValidUserCache)
                {
                    _logger.LogInfo($"User {userId} already has valid cached recommendations for all types");
                    return;
                }

                // Clear old cached recommendations for this user
                await _recommendationRepository.ClearCachedRecommendationsAsync(userId);

                var cachedRecommendations = new List<CachedRecommendation>();

                // Compute Events recommendations
                if (!hasValidEventCache)
                {
                    var eventRecommendations = await _mediator.Send(new GetRecommendedEventsQuery(userId, 20));
                    if (eventRecommendations.IsError == false)
                    {
                        foreach (var evt in eventRecommendations.Value)
                        {
                            cachedRecommendations.Add(new CachedRecommendation
                            {
                                UserId = userId,
                                RecommendationType = RecommendationType.Event.ToString(),
                                EventId = evt.Id,
                                ComputedAt = DateTime.UtcNow,
                                IsActive = true
                            });
                        }
                        _logger.LogInfo($"Computed {eventRecommendations.Value.Count} event recommendations for user {userId}");
                    }
                }

                // Compute BlindDate recommendations
                if (!hasValidBlindDateCache)
                {
                    var blindDateRecommendations = await _mediator.Send(new GetRecommendedBlindDatesQuery(userId, 15));
                    if (blindDateRecommendations.IsError == false)
                    {
                        foreach (var bd in blindDateRecommendations.Value)
                        {
                            cachedRecommendations.Add(new CachedRecommendation
                            {
                                UserId = userId,
                                RecommendationType = RecommendationType.BlindDate.ToString(),
                                BlindDateId = bd.Id,
                                ComputedAt = DateTime.UtcNow,
                                IsActive = true
                            });
                        }
                        _logger.LogInfo($"Computed {blindDateRecommendations.Value.Count} blind date recommendations for user {userId}");
                    }
                }

                // Compute SpeedDate recommendations
                if (!hasValidSpeedDateCache)
                {
                    var speedDateRecommendations = await _mediator.Send(new GetRecommendedSpeedDatesQuery(userId, 15));
                    if (speedDateRecommendations.IsError == false)
                    {
                        foreach (var sd in speedDateRecommendations.Value)
                        {
                            cachedRecommendations.Add(new CachedRecommendation
                            {
                                UserId = userId,
                                RecommendationType = RecommendationType.SpeedDate.ToString(),
                                SpeedDatingEventId = sd.Id,
                                ComputedAt = DateTime.UtcNow,
                                IsActive = true
                            });
                        }
                        _logger.LogInfo($"Computed {speedDateRecommendations.Value.Count} speed date recommendations for user {userId}");
                    }
                }

                // Compute User recommendations
                if (!hasValidUserCache)
                {
                    var userRecommendations = await _mediator.Send(new GetRecommendedUsersQuery(userId, 25));
                    if (userRecommendations.IsError == false)
                    {
                        foreach (var user in userRecommendations.Value)
                        {
                            cachedRecommendations.Add(new CachedRecommendation
                            {
                                UserId = userId,
                                RecommendationType = RecommendationType.User.ToString(),
                                RecommendedUserId = user.Id,
                                ComputedAt = DateTime.UtcNow,
                                IsActive = true
                            });
                        }
                        _logger.LogInfo($"Computed {userRecommendations.Value.Count} user recommendations for user {userId}");
                    }
                }

                // Save all cached recommendations
                if (cachedRecommendations.Any())
                {
                    await _recommendationRepository.SaveCachedRecommendationsAsync(userId, cachedRecommendations);
                    _logger.LogInfo($"Successfully cached {cachedRecommendations.Count} total recommendations for user {userId}");
                }
                else
                {
                    _logger.LogInfo($"No new recommendations to cache for user {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to pre-compute recommendations for user {userId}: {ex.Message}");
                // Don't rethrow - we want to continue processing other users
            }
        }
    }
}
