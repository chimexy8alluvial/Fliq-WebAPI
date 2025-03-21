

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Infrastructure.Persistence.Repositories;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Commands.SpeedDating
{
    public record JoinSpeedDatingEventCommand(int UserId, int SpeedDateId) : IRequest<ErrorOr<Unit>>;
    public class JoinSpeedDatingEventCommandHandler : IRequestHandler<JoinSpeedDatingEventCommand, ErrorOr<Unit>>
    {
        private readonly ISpeedDatingEventRepository _speedDateRepository;
        private readonly ISpeedDateParticipantRepository _speedDateParticipantRepository;
        private readonly ILoggerManager _loggerManager;

        public JoinSpeedDatingEventCommandHandler(
            ISpeedDatingEventRepository speedDateRepository,
            ISpeedDateParticipantRepository speedDateParticipantRepository,
            ILoggerManager loggerManager)
        {
            _speedDateRepository = speedDateRepository;
            _speedDateParticipantRepository = speedDateParticipantRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<Unit>> Handle(JoinSpeedDatingEventCommand command, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"User {command.UserId} attempting to a speed date event {command.SpeedDateId}");

            // Validate speed date existence
            var speedDate = await _speedDateRepository.GetByIdAsync(command.SpeedDateId);
            if (speedDate == null)
            {
                _loggerManager.LogWarn($"Speed date event {command.SpeedDateId} not found.");
                return Errors.Dating.BlindDateNotFound;
            }

            // Ensure session has not ended
            if (speedDate.EndSessionTime.HasValue && speedDate.EndSessionTime < DateTime.UtcNow)
            {
                _loggerManager.LogWarn($"Speed date event {command.SpeedDateId} has already ended.");
                return Errors.Dating.BlindDateSessionEnded;
            }

            // Check if user already joined
            var existingParticipant = await _speedDateParticipantRepository.GetByUserAndSpeedDateId(command.UserId, command.SpeedDateId);
            if (existingParticipant != null)
            {
                _loggerManager.LogWarn($"User {command.UserId} is already a participant in blind date {command.SpeedDateId}.");
                return Errors.Dating.AlreadyJoined;
            }

            // Check participant limit
            int currentParticipants = await _speedDateParticipantRepository.CountByBlindDateId(command.SpeedDateId);
            if (currentParticipants >= speedDate.MaxParticipants)
            {
                _loggerManager.LogWarn($"Blind date {command.SpeedDateId} has reached its maximum participants.");
                return Errors.Dating.BlindDateFull;
            }

            // Add new participant
            var newParticipant = new Domain.Entities.DatingEnvironment.SpeedDates.SpeedDatingParticipant
            {
                SpeedDatingEventId = speedDate.Id,
                UserId = command.UserId,
                IsCreator = false // Since this is a joiner
            };

            await _speedDateParticipantRepository.AddAsync(newParticipant);
            _loggerManager.LogInfo($"User {command.UserId} successfully joined speed date {command.SpeedDateId}.");

            return Unit.Value;
        }
    }
}
