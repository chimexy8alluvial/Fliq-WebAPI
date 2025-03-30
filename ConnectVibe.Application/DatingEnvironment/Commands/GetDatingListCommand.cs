
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Contracts.Dating;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using Fliq.Domain.Common.Errors;
using MediatR;
using System.Data.SqlTypes;

namespace Fliq.Application.DatingEnvironment.Commands
{
    public record GetDatingListCommand(
        int Page,
        int PageSize,
        string? Title,
        DatingType? Type,
        string? CreatedBy,
        string? SubscriptionType,
        TimeSpan? Duration,
        DateTime? DateCreatedFrom, 
        DateTime? DateCreatedTo 
    ) : IRequest<ErrorOr<GetDatingListResponse>>;
    public class GetDatingListCommandHandler : IRequestHandler<GetDatingListCommand, ErrorOr<GetDatingListResponse>>
    {
        private readonly ILoggerManager _logger;
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly ISpeedDatingEventRepository _speedDatingEventRepository;
        public GetDatingListCommandHandler(ILoggerManager logger, ISpeedDatingEventRepository speedDatingEventRepository, IBlindDateRepository blindDateRepository)
        {
            _logger = logger;
            _blindDateRepository = blindDateRepository;
            _speedDatingEventRepository = speedDatingEventRepository;
        }

        public async Task<ErrorOr<GetDatingListResponse>> Handle(GetDatingListCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Get dating list command received. Type: {command.Type} Page: {command.Page}, PageSize: {command.PageSize}");

            if (command.Page <= 0)
            {
                _logger.LogError("Invalid page number provided: Page must be greater than 0");
                return Errors.Dating.InvalidPaginationPage;
            }
            if (command.PageSize <= 0)
            {
                _logger.LogError("Invalid page size provided: PageSize must be greater than 0");
                return Errors.Dating.InvalidPaginationPageSize;
            }

            if (command.DateCreatedFrom.HasValue &&
                (command.DateCreatedFrom.Value < SqlDateTime.MinValue.Value || command.DateCreatedFrom.Value > SqlDateTime.MaxValue.Value))
            {
                _logger.LogError($"Invalid DateCreatedFrom provided: {command.DateCreatedFrom.Value} is outside SQL Server DATETIME range (1753-9999)");
                return Errors.Dating.InvalidDateCreatedFromRange;
            }
            if (command.DateCreatedTo.HasValue &&
                (command.DateCreatedTo.Value < SqlDateTime.MinValue.Value || command.DateCreatedTo.Value > SqlDateTime.MaxValue.Value))
            {
                _logger.LogError($"Invalid DateCreatedTo provided: {command.DateCreatedTo.Value} is outside SQL Server DATETIME range (1753-9999)");
                return Errors.Dating.InvalidDateCreatedToRange;
            }
            if (command.DateCreatedFrom.HasValue && command.DateCreatedTo.HasValue &&
                command.DateCreatedFrom.Value > command.DateCreatedTo.Value)
            {
                _logger.LogError($"Invalid date range: DateCreatedFrom ({command.DateCreatedFrom.Value}) is after DateCreatedTo ({command.DateCreatedTo.Value})");
                return Errors.Dating.InvalidDateRangeOrder;
            }

            if (command.Duration.HasValue && command.Duration.Value < TimeSpan.Zero)
            {
                _logger.LogError($"Invalid Duration provided: {command.Duration.Value} cannot be negative");
                return Errors.Dating.InvalidDuration;
            }


            var allEvents = new List<DatingListItem>();
            int totalCount = 0;

            if (!command.Type.HasValue || command.Type == DatingType.BlindDating)
            {
                _logger.LogInfo("Fetching blind dating events"); 
                var (blindDates, blindCount) = await _blindDateRepository.GetAllFilteredListAsync(
                command.Title, command.Type, command.Duration, command.SubscriptionType, command.DateCreatedFrom, command.DateCreatedTo, command.CreatedBy, command.Page, command.PageSize);

                if (blindDates == null || !blindDates.Any())
                {
                    _logger.LogWarn("No blind dating events found matching the filters");
                }
                else
                {
                    allEvents.AddRange(blindDates);
                    totalCount += blindCount;
                    _logger.LogInfo($"Retrieved {blindDates.Count} blind dating events, total count: {blindCount}");
                }
            }
            if (!command.Type.HasValue || command.Type == DatingType.SpeedDating)
            {
                _logger.LogInfo("Fetching speed dating events");
                var (speedDates, speedCount) = await _speedDatingEventRepository.GetAllFilteredListAsync(
                command.Title, command.Type, command.Duration, command.SubscriptionType, command.DateCreatedFrom, command.DateCreatedTo, command.CreatedBy, command.Page, command.PageSize);

                if (speedDates == null || !speedDates.Any())
                {
                    _logger.LogWarn("No speed dating events found matching the filters");
                }
                else
                {
                    allEvents.AddRange(speedDates);
                    totalCount += speedCount;
                    _logger.LogInfo($"Retrieved {speedDates.Count} speed dating events, total count: {speedCount}");
                }
            }

            if (!allEvents.Any())
            {
                _logger.LogError("No dating events found matching the provided filters");
                return Errors.Dating.NoEventsFound;
            }

            var pagedEvents = allEvents.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize)
            .Select(e => new DatingListItem
            {
                //Id = e.Id,
                Title = e.Title,
                DateCreated = e.DateCreated,
                Type = e.Type,
                CreatedBy = e.CreatedBy,
                SubscriptionType = e.SubscriptionType,
                Duration = e.Duration
            }).ToList();

            _logger.LogInfo($"Retrieved {pagedEvents.Count} events out of {totalCount} total matching filters");

            return new GetDatingListResponse(pagedEvents, totalCount, command.Page, command.PageSize);
        }
    }
}


