

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common.BlindDates;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Commands.SpeedDating
{
    public record EndSpeedDatingEventCommand(int UserId, int SpeedDateId) : IRequest<ErrorOr<EndBlindDateResult>>;

    public class EndSpeedDateCommandHandler : IRequestHandler<EndSpeedDatingEventCommand, ErrorOr<EndBlindDateResult>>
    {
        private readonly ISpeedDatingEventRepository _speedDateRepository;
        private readonly ISpeedDateParticipantRepository _speedDateParticipantRepository;
        private readonly ILoggerManager _loggerManager;

        public EndSpeedDateCommandHandler(
            ISpeedDatingEventRepository speedDateRepository,
            ISpeedDateParticipantRepository speedDateParticipantRepository,
            ILoggerManager loggerManager)
        {
            _speedDateRepository = speedDateRepository;
            _speedDateParticipantRepository = speedDateParticipantRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<EndBlindDateResult>> Handle(EndSpeedDatingEventCommand command, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"User {command.UserId} attempting to end blind date {command.SpeedDateId}");

            // Step 1: Retrieve the blind date session
            var speedDate = await _speedDateRepository.GetByIdAsync(command.SpeedDateId);
            if (speedDate == null)
            {
                _loggerManager.LogWarn($"Blind date {command.SpeedDateId} not found.");
                return Errors.Dating.BlindDateNotFound;
            }

            // Step 2: Ensure the session is ongoing
            if (speedDate.Status != DateStatus.Ongoing)
            {
                _loggerManager.LogWarn($"Speed date session {command.SpeedDateId} is not currently ongoing.");
                return Errors.Dating.BlindDateNotOngoing;
            }

            // Step 3: Ensure the user is the creator
            var creator = await _speedDateParticipantRepository.GetCreatorByBlindDateId(command.SpeedDateId);
            if (creator == null || creator.UserId != command.UserId)
            {
                _loggerManager.LogWarn($"User {command.UserId} is not the creator of speed date {command.SpeedDateId}.");
                return Errors.Dating.NotSessionCreator;
            }

            // Step 4: End the session
            var sessionEndTime = DateTime.UtcNow;
            speedDate.EndSessionTime = sessionEndTime;
            speedDate.Status = DateStatus.Completed;
            await _speedDateRepository.UpdateAsync(speedDate);

            _loggerManager.LogInfo($"Speed date session {command.SpeedDateId} ended successfully by user {command.UserId}.");

            return new EndBlindDateResult(sessionEndTime);
        }
    }
   
   
}
