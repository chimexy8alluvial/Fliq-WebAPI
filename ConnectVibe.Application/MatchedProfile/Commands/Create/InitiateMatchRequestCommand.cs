using ErrorOr;
using Fliq.Application.Common.Helpers;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
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
        private readonly ILoggerManager _logger;

        public InitiateMatchRequestCommandHandler(IMapper mapper, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository, IMediator mediator, ILoggerManager logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateMatchProfileResult>> Handle(InitiateMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _logger.LogInfo("InitiateMatchRequestCommandHandler called");
            var requestedUser = _userRepository.GetUserById(command.MatchReceiverUserId);
            if (requestedUser == null)
            {
                _logger.LogError("User not found");
                return Errors.User.UserNotFound;
            }
            var matchInitiator = _userRepository.GetUserByIdIncludingProfile(command.MatchInitiatorUserId);
            if (matchInitiator == null)
            {
                _logger.LogError("User not found");
                return Errors.User.UserNotFound;
            }
            var matchProfile = _mapper.Map<MatchRequest>(command);

            matchProfile.MatchRequestStatus = MatchRequestStatus.Pending;

            _matchProfileRepository.Add(matchProfile);

            _logger.LogInfo("MatchProfile created");

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

            _logger.LogInfo("MatchRequestEvent notification sent");
            return new CreateMatchProfileResult(matchProfile.MatchInitiatorUserId,
                matchInitiator.FirstName,
                pictureUrl,
                initiatorAge);
        }
    }
}