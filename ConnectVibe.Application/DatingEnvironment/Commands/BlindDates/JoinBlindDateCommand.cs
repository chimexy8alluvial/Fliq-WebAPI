using ErrorOr;
using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.DatingEnvironment.Commands.BlindDates
{
    public record JoinBlindDateCommand(int UserId, int BlindDateId) : IRequest<ErrorOr<Unit>>;
    public class JoinBlindDateCommandHandler : IRequestHandler<JoinBlindDateCommand, ErrorOr<Unit>>
    {
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly IBlindDateParticipantRepository _blindDateParticipantRepository;
        private readonly ILoggerManager _loggerManager;

        public JoinBlindDateCommandHandler(
            IBlindDateRepository blindDateRepository,
            IBlindDateParticipantRepository blindDateParticipantRepository,
            ILoggerManager loggerManager,
            IHubContext<BlindDateHub> hubContext)
        {
            _blindDateRepository = blindDateRepository;
            _blindDateParticipantRepository = blindDateParticipantRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<Unit>> Handle(JoinBlindDateCommand command, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"User {command.UserId} attempting to join blind date {command.BlindDateId}");

            // Validate blind date existence
            var blindDate = await _blindDateRepository.GetByIdAsync(command.BlindDateId);
            if (blindDate == null)
            {
                _loggerManager.LogWarn($"Blind date {command.BlindDateId} not found.");
                return Errors.Dating.BlindDateNotFound;
            }

            // Ensure session has not ended
            if (blindDate.SessionEndTime.HasValue && blindDate.SessionEndTime < DateTime.UtcNow)
            {
                _loggerManager.LogWarn($"Blind date {command.BlindDateId} has already ended.");
                return Errors.Dating.BlindDateSessionEnded;
            }

            // Check if user already joined
            var existingParticipant = await _blindDateParticipantRepository.GetByUserAndBlindDateId(command.UserId, command.BlindDateId);
            if (existingParticipant != null)
            {
                _loggerManager.LogWarn($"User {command.UserId} is already a participant in blind date {command.BlindDateId}.");
                return Errors.Dating.AlreadyJoined;
            }

            // Check participant limit
            int currentParticipants = await _blindDateParticipantRepository.CountByBlindDateId(command.BlindDateId);
            if (currentParticipants >= blindDate.NumberOfParticipants)
            {
                _loggerManager.LogWarn($"Blind date {command.BlindDateId} has reached its maximum participants.");
                return Errors.Dating.BlindDateFull;
            }

            // Add new participant
            var newParticipant = new BlindDateParticipant
            {
                BlindDateId = blindDate.Id,
                UserId = command.UserId,
                IsCreator = false // Since this is a joiner
            };

            await _blindDateParticipantRepository.AddAsync(newParticipant);
            _loggerManager.LogInfo($"User {command.UserId} successfully joined blind date {command.BlindDateId}.");

            return Unit.Value;
        }
    }
}
