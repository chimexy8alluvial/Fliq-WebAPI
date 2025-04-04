using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands;
using Fliq.Domain.Entities.DatingEnvironment;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Infrastructure.Persistence.Repositories;

namespace Fliq.Application.DatingEnvironment.Helper
{
    public static class DatingListDistributionHelper
    {
        public static async Task<(List<DatingListItems> Events, int TotalCount)> DistributeAndFetchDatingEvents(
            GetDatingListCommand command,
            IBlindDateRepository blindDateRepository,
            ISpeedDatingEventRepository speedDatingEventRepository,
            ILoggerManager logger,
            CancellationToken cancellationToken)
        {
            // Fetch total counts to determine distribution
            int totalBlindCount = 0;
            int totalSpeedCount = 0;

            if (!command.Type.HasValue || command.Type == DatingType.BlindDating)
            {
                var (_, blindCount) = await blindDateRepository.GetAllFilteredListAsync(
                    command.Title, command.Type, command.Duration, command.SubscriptionType,
                    command.DateCreatedFrom, command.DateCreatedTo, command.CreatedBy,
                    Page: 1, PageSize: 1);
                totalBlindCount = blindCount;
            }

            if (!command.Type.HasValue || command.Type == DatingType.SpeedDating)
            {
                var (_, speedCount) = await speedDatingEventRepository.GetAllFilteredListAsync(
                    command.Title, command.Type, command.Duration, command.SubscriptionType,
                    command.DateCreatedFrom, command.DateCreatedTo, command.CreatedBy,
                    Page: 1, PageSize: 1);
                totalSpeedCount = speedCount;
            }

            int totalCount = totalBlindCount + totalSpeedCount;
            if (totalCount == 0)
            {
                logger.LogError("No dating events found matching the provided filters");
                return (new List<DatingListItems>(), 0);
            }

            // Calculate proportional split based on total available events
            int targetBlindPerPage, targetSpeedPerPage;

            if (totalBlindCount == 0)
            {
                // If no blind dating events, use all slots for speed dating
                targetBlindPerPage = 0;
                targetSpeedPerPage = command.PageSize;
            }
            else if (totalSpeedCount == 0)
            {
                // If no speed dating events, use all slots for blind dating
                targetBlindPerPage = command.PageSize;
                targetSpeedPerPage = 0;
            }
            else
            {
                // Both types exist, distribute proportionally
                double blindRatio = (double)totalBlindCount / totalCount;
                targetBlindPerPage = (int)Math.Ceiling(command.PageSize * blindRatio);
                targetSpeedPerPage = command.PageSize - targetBlindPerPage;
            }

            // Calculate start indices for each type
            int blindStart = (command.Page - 1) * targetBlindPerPage;
            int speedStart = (command.Page - 1) * targetSpeedPerPage;

            var allEvents = new List<DatingListItems>();

            // Fetch BlindDating events
            if ((!command.Type.HasValue || command.Type == DatingType.BlindDating) && targetBlindPerPage > 0 && blindStart < totalBlindCount)
            {
                logger.LogInfo($"Fetching {targetBlindPerPage} blind dating events for page {command.Page}");
                int blindPage = (blindStart / targetBlindPerPage) + 1;

                var (blindDates, _) = await blindDateRepository.GetAllFilteredListAsync(
                    command.Title, command.Type, command.Duration, command.SubscriptionType,
                    command.DateCreatedFrom, command.DateCreatedTo, command.CreatedBy,
                    Page: blindPage, PageSize: targetBlindPerPage);

                if (blindDates != null && blindDates.Any())
                {
                    allEvents.AddRange(blindDates);
                    logger.LogInfo($"Retrieved {blindDates.Count} blind dating events for page {command.Page}");
                }
            }

            // Fetch SpeedDating events
            if ((!command.Type.HasValue || command.Type == DatingType.SpeedDating) && targetSpeedPerPage > 0 && speedStart < totalSpeedCount)
            {
                logger.LogInfo($"Fetching {targetSpeedPerPage} speed dating events for page {command.Page}");
                int speedPage = (speedStart / targetSpeedPerPage) + 1;

                var (speedDates, _) = await speedDatingEventRepository.GetAllFilteredListAsync(
                    command.Title, command.Type, command.Duration, command.SubscriptionType,
                    command.DateCreatedFrom, command.DateCreatedTo, command.CreatedBy,
                    Page: speedPage, PageSize: targetSpeedPerPage);

                if (speedDates != null && speedDates.Any())
                {
                    allEvents.AddRange(speedDates);
                    logger.LogInfo($"Retrieved {speedDates.Count} speed dating events for page {command.Page}");
                }
            }

            return (allEvents, totalCount);
        }
    }
}