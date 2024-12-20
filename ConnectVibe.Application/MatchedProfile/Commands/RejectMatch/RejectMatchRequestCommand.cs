using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Domain.Common.Errors;
using MediatR;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Application.Common.Interfaces.Services;

namespace Fliq.Application.MatchedProfile.Commands.RejectMatch
{
    public class RejectMatchRequestCommand : IRequest<ErrorOr<RejectMatchResult>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class RejectMatchRequestComandHandler : IRequestHandler<RejectMatchRequestCommand, ErrorOr<RejectMatchResult>>
    {
        private readonly IMatchProfileRepository _matchProfileRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerManager _logger;

        public RejectMatchRequestComandHandler(IMatchProfileRepository matchProfileRepository, IMediator mediator, ILoggerManager logger)
        {
            _matchProfileRepository = matchProfileRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ErrorOr<RejectMatchResult>> Handle(RejectMatchRequestCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Rejecting match request.");
            await Task.CompletedTask;

            var matchRequest = _matchProfileRepository.GetMatchRequestById(command.Id);
            if (matchRequest == null)
            {
                return Errors.MatchRequest.RequestNotFound;
            }

            if (matchRequest.UserId != command.UserId)
            {
                return Errors.MatchRequest.UnauthorizedAttempt;
            }
            if (matchRequest.matchRequestStatus == MatchRequestStatus.Rejected)
            {
                return Errors.MatchRequest.AlreadyAccepted;
            }

            matchRequest.matchRequestStatus = MatchRequestStatus.Rejected;
            _matchProfileRepository.Update(matchRequest);

            _logger.LogInfo("Match request rejected.");
            // Trigger MatchRejectedEvent notification
            await _mediator.Publish(new MatchRejectedEvent(command.UserId));

            return new RejectMatchResult(matchRequest.MatchInitiatorUserId,
                 matchRequest.matchRequestStatus);
        }
    }
}