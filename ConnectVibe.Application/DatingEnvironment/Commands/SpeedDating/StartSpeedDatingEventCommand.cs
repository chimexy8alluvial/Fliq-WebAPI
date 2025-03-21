

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common.SpeedDating;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using MediatR;


namespace Fliq.Application.DatingEnvironment.Commands.SpeedDating
{
    public record StartSpeedDatingEventCommand(int UserId, int SpeedDateId) : IRequest<ErrorOr<StartSpeedDatingEventResult>>;

    public class StartSpeedDatingEventCommandHandler : IRequestHandler<StartSpeedDatingEventCommand, ErrorOr<StartSpeedDatingEventResult>>
    {
        private readonly ISpeedDatingEventRepository _speedDateRepository;
        private readonly ISpeedDateParticipantRepository _speedDateParticipantRepository;
        private readonly ILoggerManager _loggerManager;


        public StartSpeedDatingEventCommandHandler(
            ISpeedDatingEventRepository speedDateRepository,
            ISpeedDateParticipantRepository speedDateParticipantRepository,
            ILoggerManager loggerManager)
        {
            _speedDateRepository = speedDateRepository;
            _speedDateParticipantRepository = speedDateParticipantRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<StartSpeedDatingEventResult>> Handle(StartSpeedDatingEventCommand command, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"User {command.UserId} is attempting to start blind date {command.SpeedDateId}");

            // Step 1: Retrieve the blind date session
            var speedDate = await _speedDateRepository.GetByIdAsync(command.SpeedDateId);
            if (speedDate == null)
            {
                _loggerManager.LogWarn($"Blind date session {command.SpeedDateId} not found.");
                return Errors.Dating.SpeedDateNotFound;
            }

            // Step 2: Ensure the user is the creator
            var creator = await _speedDateParticipantRepository.GetCreatorByBlindDateId(command.SpeedDateId);
            if (creator == null || creator.UserId != command.UserId)
            {
                _loggerManager.LogWarn($"User {command.UserId} is not the creator of speed date event {command.SpeedDateId}.");
                return Errors.Dating.NotSessionCreator;
            }

            // Step 3: Ensure the session hasn't started or ended
            if (speedDate.Status is DateStatus.Ongoing)
            {
                _loggerManager.LogWarn($"Speed date session {command.SpeedDateId} has already started.");
                return Errors.Dating.BlindDateAlreadyStarted;
            }

            if (speedDate.Status is DateStatus.Completed || speedDate.Status is DateStatus.Cancelled)
            {
                _loggerManager.LogWarn($"Speed date session {command.SpeedDateId} has already ended.");
                return Errors.Dating.BlindDateAlreadyEnded;
            }

            if (speedDate.Status == DateStatus.Cancelled)
            {
                _loggerManager.LogWarn($"Speed date session  {command.SpeedDateId} is cancelled and cannot be started.");
                return Errors.Dating.BlindDateCancelled;
            }
            // Step 4: Start the session
            var sessionStartTime = DateTime.UtcNow;
            speedDate.StartSessionTime = sessionStartTime;
            speedDate.Status = DateStatus.Ongoing;
            await _speedDateRepository.UpdateAsync(speedDate);

            _loggerManager.LogInfo($"Speed date session {command.SpeedDateId} started successfully by user {command.UserId}.");

            return new StartSpeedDatingEventResult(sessionStartTime);
        }
    }
}
