using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common.BlindDates;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;



namespace Fliq.Application.DatingEnvironment.Commands.BlindDates
{
    public record EndBlindDateCommand(int UserId, int BlindDateId) : IRequest<ErrorOr<EndBlindDateResult>>;

    public class EndBlindDateCommandHandler : IRequestHandler<EndBlindDateCommand, ErrorOr<EndBlindDateResult>>
    {
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly IBlindDateParticipantRepository _blindDateParticipantRepository;
        private readonly ILoggerManager _loggerManager;

        public EndBlindDateCommandHandler(
            IBlindDateRepository blindDateRepository,
            IBlindDateParticipantRepository blindDateParticipantRepository,
            ILoggerManager loggerManager)
        {
            _blindDateRepository = blindDateRepository;
            _blindDateParticipantRepository = blindDateParticipantRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<EndBlindDateResult>> Handle(EndBlindDateCommand command, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"User {command.UserId} attempting to end blind date {command.BlindDateId}");

            // Step 1: Retrieve the blind date session
            var blindDate = await _blindDateRepository.GetByIdAsync(command.BlindDateId);
            if (blindDate == null)
            {
                _loggerManager.LogWarn($"Blind date {command.BlindDateId} not found.");
                return Errors.Dating.BlindDateNotFound;
            }

            // Step 2: Ensure the session is ongoing
            if (blindDate.Status != DateStatus.Ongoing)
            {
                _loggerManager.LogWarn($"Blind date session {command.BlindDateId} is not currently ongoing.");
                return Errors.Dating.BlindDateNotOngoing;
            }

            // Step 3: Ensure the user is the creator
            var creator = await _blindDateParticipantRepository.GetCreatorByBlindDateId(command.BlindDateId);
            if (creator == null || creator.UserId != command.UserId)
            {
                _loggerManager.LogWarn($"User {command.UserId} is not the creator of blind date {command.BlindDateId}.");
                return Errors.Dating.NotSessionCreator;
            }

            // Step 4: End the session
           var sessionEndTime = DateTime.UtcNow;
            blindDate.SessionEndTime = sessionEndTime;
           blindDate.Status = DateStatus.Completed;
            await _blindDateRepository.UpdateAsync(blindDate);

            _loggerManager.LogInfo($"Blind date session {command.BlindDateId} ended successfully by user {command.UserId}.");

            return new EndBlindDateResult(sessionEndTime);
        }
    }
}
