using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.SchedulingServices.QuartzJobs
{
    public class CleanupOldRecommendationsJob : IJob
    {
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ILoggerManager _logger;

        public CleanupOldRecommendationsJob(
            IRecommendationRepository recommendationRepository,
            ILoggerManager logger)
        {
            _recommendationRepository = recommendationRepository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Starting cleanup of old cached recommendations");

            try
            {
                var jobDataMap = context.JobDetail.JobDataMap;
                var maxAgeInDays = jobDataMap.GetIntValue("MaxAgeInDays") > 0 ? jobDataMap.GetIntValue("MaxAgeInDays") : 7;

                await CleanupOldRecommendations(maxAgeInDays);

                _logger.LogInfo("Cleanup of old cached recommendations completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in cleanup old recommendations job: {ex.Message}");

                var jobException = new JobExecutionException(ex)
                {
                    RefireImmediately = false
                };
                throw jobException;
            }
        }

        private async Task CleanupOldRecommendations(int maxAgeInDays)
        {
            await _recommendationRepository.CleanupOldCachedRecommendationsAsync(DateTime.UtcNow.AddDays(-maxAgeInDays));
            _logger.LogInfo($"Cleaned up cached recommendations older than {maxAgeInDays} days");
        }
    }
}
