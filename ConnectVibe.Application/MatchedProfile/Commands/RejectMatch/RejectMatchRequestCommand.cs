using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Domain.Common.Errors;
using MediatR;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Enums;

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
                _logger.LogWarn("Match request not found");
                return Errors.MatchRequest.RequestNotFound;
            }

            if (matchRequest.MatchReceiverUserId != command.UserId)
            {
                _logger.LogError($"User with Id --> {command.UserId} is unauthorized to reject this match request");
                return Errors.MatchRequest.UnauthorizedAttempt;
            }
            if (matchRequest.MatchRequestStatus == MatchRequestStatus.Rejected)
            {
                _logger.LogInfo($"Match request already rejected");
                return Errors.MatchRequest.AlreadyRejected;
            }

            matchRequest.MatchRequestStatus = MatchRequestStatus.Rejected;
            _matchProfileRepository.Update(matchRequest);
            _logger.LogInfo($"Match request rejected by user --> {command.UserId}");

            // Trigger MatchRejectedEvent notification
            await _mediator.Publish(new MatchRejectedEvent(command.UserId));

            return new RejectMatchResult(matchRequest.MatchInitiatorUserId,
                 matchRequest.MatchRequestStatus);
        }
    }
}