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

            var matchProfile = _matchProfileRepository.GetMatchProfileById(command.Id);
            if (matchProfile == null)
            {
                _logger.LogError("Match profile not found");
                return Errors.Profile.ProfileNotFound;
            }

            matchProfile.MatchRequestStatus = MatchRequestStatus.Accepted;
            _matchProfileRepository.Update(matchProfile);

            //trigger Accepted match event notification
            await _mediator.Publish(new MatchAcceptedEvent(command.UserId, matchProfile.MatchInitiatorUserId, command.UserId));

            _logger.LogInfo("Match request accepted");
            return new CreateAcceptMatchResult(matchProfile.MatchInitiatorUserId,
                 matchProfile.MatchRequestStatus);
        }
    }
}