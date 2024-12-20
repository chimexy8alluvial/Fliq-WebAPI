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
               return Errors.MatchRequest.RequestNotFound;
            }

            if(matchRequest.MatchReceiverUserId != command.UserId)
            {
                return Errors.MatchRequest.UnauthorizedAttempt;
            }
            if (matchRequest.MatchRequestStatus == MatchRequestStatus.Accepted)
            {
                return Errors.MatchRequest.AlreadyAccepted;
            }

            matchRequest.MatchRequestStatus = MatchRequestStatus.Accepted;
            _matchProfileRepository.Update(matchRequest);
            
            //trigger Accepted match event notification
            await _mediator.Publish(new MatchAcceptedEvent(command.UserId, matchRequest.MatchInitiatorUserId, command.UserId));

           return new CreateAcceptMatchResult(matchRequest.MatchInitiatorUserId,
                matchRequest.MatchRequestStatus);

        }
    }
}