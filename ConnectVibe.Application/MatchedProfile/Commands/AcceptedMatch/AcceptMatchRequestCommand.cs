using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.AcceptedMatch
{
    public class AcceptMatchRequestCommand : IRequest<ErrorOr<CreateAcceptMatchResult>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class AcceptMatchRequestCommandHandler : IRequestHandler<AcceptMatchRequestCommand, ErrorOr<CreateAcceptMatchResult>>
    {
        private readonly IMatchProfileRepository _matchProfileRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerManager _logger;

        public AcceptMatchRequestCommandHandler(IMatchProfileRepository matchProfileRepository, IMediator mediator, ILoggerManager logger)
        {
            _matchProfileRepository = matchProfileRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateAcceptMatchResult>> Handle(AcceptMatchRequestCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Accept Match request command received");
            await Task.CompletedTask;

            var matchRequest = _matchProfileRepository.GetMatchRequestById(command.Id);
            if (matchRequest == null)
            {
                _logger.LogWarn($"Match request with Id --> {command.Id} not found");
                return Errors.MatchRequest.RequestNotFound;
            }

            if(matchRequest.MatchReceiverUserId != command.UserId)
            {
                _logger.LogError($"User with Id --> {command.UserId} is unauthorized to accept this match request");
                return Errors.MatchRequest.UnauthorizedAttempt;
            }
            if (matchRequest.MatchRequestStatus == MatchRequestStatus.Accepted)
            {
                _logger.LogWarn($"Match request  with Id --> {command.Id} already accepted");
                return Errors.MatchRequest.AlreadyAccepted;
            }

            matchRequest.MatchRequestStatus = MatchRequestStatus.Accepted;
            _matchProfileRepository.Update(matchRequest);
            _logger.LogInfo($"Match request accepted by user --> {command.UserId}");

            //trigger Accepted match event notification
            await _mediator.Publish(new MatchAcceptedEvent(command.UserId, matchRequest.MatchInitiatorUserId, command.UserId));

           return new CreateAcceptMatchResult(matchRequest.MatchInitiatorUserId,
                matchRequest.MatchRequestStatus);

        }
    }
}