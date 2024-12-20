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
    public class InitiateMatchRequestCommand : IRequest<ErrorOr<CreateMatchRequestResult>>
    {
        public int MatchInitiatorUserId { get; set; }
        public int MatchReceiverUserId { get; set; }
    }

    public class InitiateMatchRequestCommandHandler : IRequestHandler<InitiateMatchRequestCommand, ErrorOr<CreateMatchRequestResult>>
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

        public async Task<ErrorOr<CreateMatchRequestResult>> Handle(InitiateMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var requestedUser = _userRepository.GetUserByIdIncludingProfile(command.UserId);
            if (requestedUser == null)
            {
                _logger.LogError("User not found");
                return Errors.User.UserNotFound;
            }
            var matchInitiator = _userRepository.GetUserByIdIncludingProfile(command.MatchInitiatorUserId);
            if (matchInitiator == null)
            {
                return Errors.User.UserNotFound;
            }

            //Check for existing request
            var isRequestExist = _matchProfileRepository.MatchRequestExist(command.MatchInitiatorUserId, command.UserId);
            if (isRequestExist) return Errors.MatchRequest.DuplicateRequest;

            //Create request object
            var matchRequest = _mapper.Map<MatchRequest>(command);

            matchRequest.matchRequestStatus = MatchRequestStatus.Pending;
            matchRequest.PictureUrl = matchInitiator.UserProfile?.Photos.FirstOrDefault()?.PictureUrl ?? "";
            matchRequest.Name = matchInitiator.FirstName;
            matchRequest.Age = matchInitiator.UserProfile?.DOB.CalculateAge();

            //Save request
            _matchProfileRepository.Add(matchRequest);

            _logger.LogInfo("MatchProfile created");

            var pictureUrl = matchInitiator?.UserProfile?.Photos?.FirstOrDefault()?.PictureUrl;
            var initiatorAge = matchInitiator.UserProfile.DOB.CalculateAge();
            // Trigger MatchRequestEvent notification
            //await _mediator.Publish(new MatchRequestEvent(
            //    command.MatchInitiatorUserId,
            //    command.UserId,
            //    accepterImageUrl: requestedUser?.UserProfile?.Photos?.FirstOrDefault()?.PictureUrl,
            //    initiatorImageUrl: matchRequest.PictureUrl,
            //    initiatorName: matchRequest.Name
            //));

            return new CreateMatchRequestResult(matchRequest.Id, matchRequest.UserId, matchRequest.MatchInitiatorUserId,
                matchRequest.Name,
                matchRequest.PictureUrl, matchRequest.Age, (int)matchRequest.matchRequestStatus);
        }
    }
}