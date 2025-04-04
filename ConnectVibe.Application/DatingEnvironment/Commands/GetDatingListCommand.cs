
using ErrorOr;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.DatingEnvironment;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Infrastructure.Persistence.Repositories;
using Fliq.Application.DatingEnvironment.Helper;

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
    ) : IRequest<ErrorOr<PaginationResponse<DatingListItems>>>;
    public class GetDatingListCommandHandler : IRequestHandler<GetDatingListCommand, ErrorOr<PaginationResponse<DatingListItems>>>
    {
        private readonly ILoggerManager _logger;
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly ISpeedDatingEventRepository _speedDatingEventRepository;

        public GetDatingListCommandHandler(ILoggerManager logger, IBlindDateRepository blindDateRepository, ISpeedDatingEventRepository speedDatingEventRepository)
        {
            _logger = logger;
            _blindDateRepository = blindDateRepository;
            _speedDatingEventRepository = speedDatingEventRepository;
        }

        public async Task<ErrorOr<PaginationResponse<DatingListItems>>> Handle(GetDatingListCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Get dating list command received. Type: {command.Type} Page: {command.Page}, PageSize: {command.PageSize}");

                var (events, totalCount) = await DatingListDistributionHelper.DistributeAndFetchDatingEvents(
                    command, _blindDateRepository, _speedDatingEventRepository, _logger, cancellationToken);

                if (!events.Any())
                {
                    return totalCount == 0
                        ? Error.NotFound("NoEventsFound", "No dating events found matching the provided filters")
                        : new PaginationResponse<DatingListItems>(events, totalCount, command.Page, command.PageSize);
                }

                _logger.LogInfo($"Retrieved {events.Count} events out of {totalCount} total matching filters");
                return new PaginationResponse<DatingListItems>(events, totalCount, command.Page, command.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing GetDatingListCommand: {ex.Message}");
                return Error.Failure("GetDatingListFailed", $"Failed to retrieve dating events: {ex.Message}");
            }
        }
    }
}