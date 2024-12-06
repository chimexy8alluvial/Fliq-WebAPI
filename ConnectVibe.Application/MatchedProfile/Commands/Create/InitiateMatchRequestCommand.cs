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
        public int MatchInitiatorUserId { get; set; }
        public int MatchReceiverUserId { get; set; }
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
            var requestedUser = _userRepository.GetUserById(command.MatchReceiverUserId);
            if (requestedUser == null)
            {
                return Errors.User.UserNotFound;
            }
            var matchInitiator = _userRepository.GetUserByIdIncludingProfile(command.MatchInitiatorUserId);
            var matchProfile = _mapper.Map<MatchRequest>(command);

            matchProfile.MatchRequestStatus = MatchRequestStatus.Pending;

            _matchProfileRepository.Add(matchProfile);

            var pictureUrl = matchInitiator?.UserProfile?.Photos?.FirstOrDefault()?.PictureUrl;
            var initiatorAge = matchInitiator.UserProfile.DOB.CalculateAge();
            // Trigger MatchRequestEvent notification
            await _mediator.Publish(new MatchRequestEvent(
                command.MatchInitiatorUserId,
                command.MatchReceiverUserId,
                accepterImageUrl: requestedUser?.UserProfile?.Photos?.FirstOrDefault()?.PictureUrl,
                initiatorImageUrl: pictureUrl,
                initiatorName: matchInitiator.FirstName
            ));

            return new CreateMatchProfileResult(matchProfile.MatchInitiatorUserId,
                matchInitiator.FirstName,
                pictureUrl,
                initiatorAge);
        }
    }
}