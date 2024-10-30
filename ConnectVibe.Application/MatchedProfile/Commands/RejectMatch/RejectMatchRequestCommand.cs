using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Domain.Enums;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fliq.Application.MatchedProfile.Commands.RejectMatch
{
    public class RejectMatchRequestCommand : IRequest<ErrorOr<RejectMatchResult>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class RejectMatchRequestComandHandler : IRequestHandler<RejectMatchRequestCommand, ErrorOr<RejectMatchResult>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IUserRepository _userRepository;
        private readonly IMatchProfileRepository _matchProfileRepository;

        public RejectMatchRequestComandHandler(IMapper mapper, IImageService imageService, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository)
        {
            _mapper = mapper;
            _imageService = imageService;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;
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

            return new RejectMatchResult(matchProfile.MatchInitiatorUserId,
                 matchProfile.matchRequestStatus);
        }
    }
}
