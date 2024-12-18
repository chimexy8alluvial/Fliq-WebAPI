using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.AcceptedMatch
{
    public class AcceptMatchRequestCommand : IRequest<ErrorOr<CreateAcceptMatchResult>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //public int MatchInitiatorUserId { get; set; }
    }

    public class AcceptMatchRequestCommandHandler : IRequestHandler<AcceptMatchRequestCommand, ErrorOr<CreateAcceptMatchResult>>
    {
        private readonly IMatchProfileRepository _matchProfileRepository;
        private readonly IMediator _mediator;

        public AcceptMatchRequestCommandHandler(IMatchProfileRepository matchProfileRepository, IMediator mediator)
        {
            _matchProfileRepository = matchProfileRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<CreateAcceptMatchResult>> Handle(AcceptMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var matchRequest = _matchProfileRepository.GetMatchRequestById(command.Id);
            if (matchRequest == null)
            {
               return Errors.MatchRequest.RequestNotFound;
            }

            if(matchRequest.UserId != command.UserId)
            {
                return Errors.MatchRequest.UnauthorizedAttempt;
            }
            if (matchRequest.matchRequestStatus == MatchRequestStatus.Accepted)
            {
                return Errors.MatchRequest.AlreadyAccepted;
            }

            matchRequest.matchRequestStatus = MatchRequestStatus.Accepted;
            _matchProfileRepository.Update(matchRequest);
            
            //trigger Accepted match event notification
            await _mediator.Publish(new MatchAcceptedEvent(command.UserId, matchRequest.MatchInitiatorUserId, command.UserId));

           return new CreateAcceptMatchResult(matchRequest.MatchInitiatorUserId,
                matchRequest.matchRequestStatus);

        }
    }
}
