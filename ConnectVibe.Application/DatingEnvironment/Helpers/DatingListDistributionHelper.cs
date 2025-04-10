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

            // Calculate equal distribution based on total available events
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
                // Both types exist, distribute equally
                targetBlindPerPage = command.PageSize / 2;
                targetSpeedPerPage = command.PageSize - targetBlindPerPage;

                // Calculate remaining events of each type for this page
                int remainingBlindEvents = Math.Max(0, totalBlindCount - ((command.Page - 1) * targetBlindPerPage));
                int remainingSpeedEvents = Math.Max(0, totalSpeedCount - ((command.Page - 1) * targetSpeedPerPage));

                if (remainingBlindEvents <= 0)
                {
                    // No more blind dating events available for this page, give all slots to speed dating
                    targetBlindPerPage = 0;
                    targetSpeedPerPage = command.PageSize;
                }
                else if (remainingSpeedEvents <= 0)
                {
                    // No more speed dating events available for this page, give all slots to blind dating
                    targetBlindPerPage = command.PageSize;
                    targetSpeedPerPage = 0;
                }
                else if (remainingBlindEvents < targetBlindPerPage)
                {
                    // Not enough blind dating events left to fill the target, adjust allocation
                    targetBlindPerPage = remainingBlindEvents;
                    targetSpeedPerPage = command.PageSize - targetBlindPerPage;
                }
                else if (remainingSpeedEvents < targetSpeedPerPage)
                {
                    // Not enough speed dating events left to fill the target, adjust allocation
                    targetSpeedPerPage = remainingSpeedEvents;
                    targetBlindPerPage = command.PageSize - targetSpeedPerPage;
                }

                logger.LogInfo($"Page {command.Page}: Distributing as {targetBlindPerPage} blind dating and {targetSpeedPerPage} speed dating events");
            }

            // Calculate start indices for each type
            int blindStart = (command.Page - 1) * (command.PageSize / 2);
            int speedStart = (command.Page - 1) * (command.PageSize / 2);

            // Adjust for uneven page size
            if (command.PageSize % 2 != 0)
            {
                // Add extra slot to blind dating for odd-numbered pages, speed dating for even-numbered pages
                if (command.Page % 2 == 1)
                {
                    blindStart = (command.Page - 1) * ((command.PageSize / 2) + 1) +
                                 (command.Page - 1) / 2;
                    speedStart = (command.Page - 1) * (command.PageSize / 2) -
                                 (command.Page - 1) / 2;
                }
                else
                {
                    blindStart = (command.Page - 1) * (command.PageSize / 2) -
                                 ((command.Page - 2) / 2);
                    speedStart = (command.Page - 1) * ((command.PageSize / 2) + 1) +
                                 ((command.Page - 2) / 2);
                }
            }

            var allEvents = new List<DatingListItems>();

            // Fetch BlindDating events
            if ((!command.Type.HasValue || command.Type == DatingType.BlindDating) && targetBlindPerPage > 0 && blindStart < totalBlindCount)
            {
                logger.LogInfo($"Fetching {targetBlindPerPage} blind dating events starting from index {blindStart}");

                // Determine the actual page number in the blind dating repository
                int blindPage = (blindStart / (command.PageSize / 2)) + 1;
                int blindOffset = blindStart % (command.PageSize / 2);

                // Adjust page and size to account for the offset
                int adjustedBlindPageSize = targetBlindPerPage + blindOffset;

                var (blindDates, _) = await blindDateRepository.GetAllFilteredListAsync(
                    command.Title, command.Type, command.Duration, command.SubscriptionType,
                    command.DateCreatedFrom, command.DateCreatedTo, command.CreatedBy,
                    Page: blindPage, PageSize: adjustedBlindPageSize);

                if (blindDates != null && blindDates.Any())
                {
                    // Skip any offset items and take only what we need
                    var selectedBlindDates = blindDates.Skip(blindOffset).Take(targetBlindPerPage).ToList();
                    allEvents.AddRange(selectedBlindDates);
                    logger.LogInfo($"Retrieved {selectedBlindDates.Count} blind dating events for page {command.Page}");
                }
            }

            // Fetch SpeedDating events
            if ((!command.Type.HasValue || command.Type == DatingType.SpeedDating) && targetSpeedPerPage > 0 && speedStart < totalSpeedCount)
            {
                logger.LogInfo($"Fetching {targetSpeedPerPage} speed dating events starting from index {speedStart}");

                // Determine the actual page number in the speed dating repository
                int speedPage = (speedStart / (command.PageSize / 2)) + 1;
                int speedOffset = speedStart % (command.PageSize / 2);

                // Adjust page and size to account for the offset
                int adjustedSpeedPageSize = targetSpeedPerPage + speedOffset;

                var (speedDates, _) = await speedDatingEventRepository.GetAllFilteredListAsync(
                    command.Title, command.Type, command.Duration, command.SubscriptionType,
                    command.DateCreatedFrom, command.DateCreatedTo, command.CreatedBy,
                    Page: speedPage, PageSize: adjustedSpeedPageSize);

                if (speedDates != null && speedDates.Any())
                {
                    // Skip any offset items and take only what we need
                    var selectedSpeedDates = speedDates.Skip(speedOffset).Take(targetSpeedPerPage).ToList();
                    allEvents.AddRange(selectedSpeedDates);
                    logger.LogInfo($"Retrieved {selectedSpeedDates.Count} speed dating events for page {command.Page}");
                }
            }

            return (allEvents, totalCount);
        }
    }
}