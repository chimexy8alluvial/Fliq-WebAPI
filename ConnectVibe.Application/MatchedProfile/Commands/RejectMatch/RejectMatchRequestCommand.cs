using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Domain.Enums;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Fliq.Application.Notifications.Common.MatchEvents;

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

        public RejectMatchRequestComandHandler(IMatchProfileRepository matchProfileRepository, IMediator mediator)
        {
            _matchProfileRepository = matchProfileRepository;
            _mediator = mediator;
        }
        public async Task<ErrorOr<RejectMatchResult>> Handle(RejectMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var matchProfile = _matchProfileRepository.GetMatchProfileById(command.Id);
            if (matchProfile == null)
            {
                return Errors.Profile.ProfileNotFound;
            }

            matchProfile.matchRequestStatus = MatchRequestStatus.Rejected;
            _matchProfileRepository.Update(matchProfile);

            // Trigger MatchRejectedEvent notification
            await _mediator.Publish(new MatchRejectedEvent(command.UserId));

            return new RejectMatchResult(matchProfile.MatchInitiatorUserId,
                 matchProfile.matchRequestStatus);
        }
    }
}
