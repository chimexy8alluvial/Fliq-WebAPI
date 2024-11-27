using ErrorOr;
using Fliq.Application.Common.Helpers;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.Create
{
    public class InitiateMatchRequestCommand : IRequest<ErrorOr<CreateMatchProfileResult>>
    {
        public int UserId { get; set; }
        public int MatchInitiatorUserId { get; set; }
    }

    public class InitiateMatchRequestCommandHandler : IRequestHandler<InitiateMatchRequestCommand, ErrorOr<CreateMatchProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMatchProfileRepository _matchProfileRepository;
        private readonly IMediator _mediator;

        public InitiateMatchRequestCommandHandler(IMapper mapper, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository, IMediator mediator)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<CreateMatchProfileResult>> Handle(InitiateMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var requestedUser = _userRepository.GetUserById(command.UserId);
            if (requestedUser == null)
            {
                return Errors.User.UserNotFound;
            }
            var matchInitiator = _userRepository.GetUserByIdIncludingProfile(command.MatchInitiatorUserId);
            var matchProfile = _mapper.Map<MatchRequest>(command);

            matchProfile.matchRequestStatus = MatchRequestStatus.Pending;
            matchProfile.PictureUrl = matchInitiator?.UserProfile?.Photos?.FirstOrDefault()?.PictureUrl;
            matchProfile.Name = matchInitiator.FirstName;
            matchProfile.Age = matchInitiator.UserProfile.DOB.CalculateAge();
            _matchProfileRepository.Add(matchProfile);

            // Trigger MatchRequestEvent notification
            await _mediator.Publish(new MatchRequestEvent(
                command.MatchInitiatorUserId,
                command.UserId,
                accepterImageUrl: requestedUser?.UserProfile?.Photos?.FirstOrDefault()?.PictureUrl,
                initiatorImageUrl: matchProfile.PictureUrl,
                initiatorName: matchProfile.Name
            ));

            return new CreateMatchProfileResult(matchProfile.MatchInitiatorUserId,
                matchProfile.Name,
                matchProfile.PictureUrl,matchProfile.Age);
        }
    }
}
