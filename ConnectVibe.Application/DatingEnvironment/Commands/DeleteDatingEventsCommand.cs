

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using MediatR;
using Fliq.Domain.Common.Errors;
using System.Data;
using DatingOptions = Fliq.Application.DatingEnvironment.Common.DatingOptions;

namespace Fliq.Application.DatingEnvironment.Commands
{
    public record DeleteDatingEventsCommand(List<DatingOptions> DatingOptions) : IRequest<ErrorOr<DeleteDatingEventsResult>>;

    public class DeleteDatingEventsCommandHandler : IRequestHandler<DeleteDatingEventsCommand, ErrorOr<DeleteDatingEventsResult>>
    {
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly ISpeedDatingEventRepository _speedDatingEventRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerManager _logger;

        public DeleteDatingEventsCommandHandler( IBlindDateRepository blindDateRepository, ISpeedDatingEventRepository speedDatingEventRepository, IMediator mediator, ILoggerManager logger)
        {
            _blindDateRepository = blindDateRepository;
            _speedDatingEventRepository = speedDatingEventRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ErrorOr<DeleteDatingEventsResult>> Handle(DeleteDatingEventsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Delete date option command received");

            if (command.DatingOptions == null || !command.DatingOptions.Any())
            {
                _logger.LogError("No dating options provided for deletion");
                return Errors.Dating.NoEventsToDelete;
            }

            var blindDateIds = command.DatingOptions.Where(d => d.DatingType == DatingType.BlindDating).Select(x => x.id).ToList();
            var speedDateIds = command.DatingOptions.Where(d => d.DatingType == DatingType.SpeedDating).Select(x => x.id).ToList();

            int blindDateDeletedCount = 0;
            int speedDateDeletedCount = 0;

            if (blindDateIds.Any())
            {
                _logger.LogInfo($"Deleting {blindDateIds.Count} blind date events with IDs: {string.Join(", ", blindDateIds)}");
                blindDateDeletedCount = await _blindDateRepository.DeleteMultipleAsync(blindDateIds);
                _logger.LogInfo($"Successfully Deleted {blindDateDeletedCount} blind date events");
            }

            if (speedDateIds.Any())
            {
                _logger.LogInfo($"Deleting {speedDateIds.Count} speed date events with IDs: {string.Join(", ", speedDateIds)}");
                speedDateDeletedCount = await _speedDatingEventRepository.DeleteMultipleAsync(speedDateIds);
                _logger.LogInfo($"Successfully deleted {speedDateDeletedCount} speed date events");
            }

            int totalDeletedCount = blindDateDeletedCount + speedDateDeletedCount;

            if (totalDeletedCount == 0)
            {
                _logger.LogError("No dating events were deleted");
                return Errors.Dating.NoEventsToDelete;
            }

            _logger.LogInfo($"Total dating events seleted: {totalDeletedCount}");
            return new DeleteDatingEventsResult(totalDeletedCount, blindDateDeletedCount, speedDateDeletedCount);
        }
    }
}