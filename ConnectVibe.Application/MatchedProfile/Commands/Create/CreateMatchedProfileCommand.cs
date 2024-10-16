using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using ConnectVibe.Application.Profile.Common;
using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities.MatchedProfile;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using Fliq.Application.MatchedProfile.Common;
using MapsterMapper;
using MediatR;
using Fliq.Application.Common.Interfaces.Persistence;

namespace Fliq.Application.MatchedProfile.Commands.Create
{
    public class CreateMatchProfileCommand : IRequest<ErrorOr<CreateMatchProfileResult>>
    {
        public int UserId { get; set; }
    }

    public class CreateMatchProfileCommandHandler : IRequestHandler<CreateMatchProfileCommand, ErrorOr<CreateMatchProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IUserRepository _userRepository;
        private readonly IMatchProfileRepository _matchProfileRepository;

        public CreateMatchProfileCommandHandler(IMapper mapper, IImageService imageService, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository)
        {
            _mapper = mapper;
            _imageService = imageService;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;

        }

        public async Task<ErrorOr<CreateMatchProfileResult>> Handle(CreateMatchProfileCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var requestedUser = _userRepository.GetUserById(command.UserId);
            //Get Logged in user
            var matchProfile = _mapper.Map<MatchProfile>(command);
            if (requestedUser == null)
            {
                return Errors.Profile.ProfileNotFound;
            }
            _matchProfileRepository.Add(matchProfile);

            return new CreateMatchProfileResult(matchProfile);
        }
    }
}
