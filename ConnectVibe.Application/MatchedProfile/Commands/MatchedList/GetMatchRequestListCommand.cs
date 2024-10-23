using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.MatchedProfile;
using MapsterMapper;
using MediatR;
using System.Collections.Generic;

namespace Fliq.Application.MatchedProfile.Commands.MatchedList
{
    public record GetMatchRequestListCommand(int UserId) : IRequest<ErrorOr<List<CreateMatchListResult>>>;
    

    public class CreateMatchListCommandHandler : IRequestHandler<GetMatchRequestListCommand, ErrorOr<List<CreateMatchListResult>>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IUserRepository _userRepository;
        private readonly IMatchProfileRepository _matchProfileRepository;

        public CreateMatchListCommandHandler(IMapper mapper, IImageService imageService, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository)
        {
            _mapper = mapper;
            _imageService = imageService;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;
        }

        public async Task<ErrorOr<List<CreateMatchListResult>>> Handle(GetMatchRequestListCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                return Errors.Profile.ProfileNotFound;
            }
            var requestId = _mapper.Map<MatchRequest>(command);
            int pageSize = 1;
            int pageNumber = 11;
            var filteredValue = await _matchProfileRepository.GetMatchListById(requestId.UserId,pageNumber,pageSize);
            
            var result = _mapper.Map<List<CreateMatchListResult>>(filteredValue);

            return result;
        }
    }
}
