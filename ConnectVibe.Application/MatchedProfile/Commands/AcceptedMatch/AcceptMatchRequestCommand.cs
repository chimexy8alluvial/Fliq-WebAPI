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
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMatchProfileRepository _matchProfileRepository;
        private readonly IMediator _mediator;

        public AcceptMatchRequestCommandHandler(IMapper mapper, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository, IMediator mediator)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<CreateAcceptMatchResult>> Handle(AcceptMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var matchProfile = _matchProfileRepository.GetMatchProfileById(command.Id);
            if (matchProfile == null)
            {
               return Errors.Profile.ProfileNotFound;
            }

            matchProfile.matchRequestStatus = MatchRequestStatus.Accepted;
            _matchProfileRepository.Update(matchProfile);
            
            //trigger Accepted match event notification
            await _mediator.Publish(new MatchAcceptedEvent(command.UserId, matchProfile.MatchInitiatorUserId, command.UserId));

           return new CreateAcceptMatchResult(matchProfile.MatchInitiatorUserId,
                matchProfile.matchRequestStatus);

        }
    }
}
