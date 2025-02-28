﻿using ErrorOr;
using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common.BlindDates;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.DatingEnvironment.Commands.BlindDates
{
    public record StartBlindDateCommand(int UserId, int BlindDateId) : IRequest<ErrorOr<StartBlindDateResult>>;

    public class StartBlindDateCommandHandler : IRequestHandler<StartBlindDateCommand, ErrorOr<StartBlindDateResult>>
    {
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly IBlindDateParticipantRepository _blindDateParticipantRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IHubContext<BlindDateHub> _hubContext;


        public StartBlindDateCommandHandler(
            IBlindDateRepository blindDateRepository,
            IBlindDateParticipantRepository blindDateParticipantRepository,
            ILoggerManager loggerManager,
            IHubContext<BlindDateHub> hubContext)
        {
            _blindDateRepository = blindDateRepository;
            _blindDateParticipantRepository = blindDateParticipantRepository;
            _loggerManager = loggerManager;
            _hubContext = hubContext;
        }

        public async Task<ErrorOr<StartBlindDateResult>> Handle(StartBlindDateCommand command, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"User {command.UserId} is attempting to start blind date {command.BlindDateId}");

            // Step 1: Retrieve the blind date session
            var blindDate = await _blindDateRepository.GetByIdAsync(command.BlindDateId);
            if (blindDate == null)
            {
                _loggerManager.LogWarn($"Blind date session {command.BlindDateId} not found.");
                return Errors.Dating.BlindDateNotFound;
            }

            // Step 2: Ensure the user is the creator
            var creator = await _blindDateParticipantRepository.GetCreatorByBlindDateId(command.BlindDateId);
            if (creator == null || creator.UserId != command.UserId)
            {
                _loggerManager.LogWarn($"User {command.UserId} is not the creator of blind date {command.BlindDateId}.");
                return Errors.Dating.NotSessionCreator;
            }

            // Step 3: Ensure the session hasn't started or ended
            if (blindDate.SessionStartTime.HasValue && blindDate.Status is BlindDateStatus.Ongoing)
            {
                _loggerManager.LogWarn($"Blind date session {command.BlindDateId} has already started.");
                return Errors.Dating.BlindDateAlreadyStarted;
            }

            if (blindDate.SessionStartTime.HasValue && (blindDate.Status is BlindDateStatus.Completed || blindDate.Status is BlindDateStatus.Cancelled))
            {
                _loggerManager.LogWarn($"Blind date session {command.BlindDateId} has already ended.");
                return Errors.Dating.BlindDateAlreadyEnded;
            }

            // Step 4: Start the session
            var sessionStartTime = DateTime.UtcNow;
            blindDate.SessionStartTime = sessionStartTime;
            blindDate.Status = BlindDateStatus.Ongoing;
            await _blindDateRepository.UpdateAsync(blindDate);

            // Notify users via signalR
            await _hubContext.Clients.Group($"BlindDate-{command.BlindDateId}").SendAsync("BlinDateStarted", command.BlindDateId, cancellationToken);

            _loggerManager.LogInfo($"Blind date session {command.BlindDateId} started successfully by user {command.UserId}.");

            return new StartBlindDateResult(sessionStartTime);
        }
    }
}
