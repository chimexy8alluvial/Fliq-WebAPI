using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Common.Pagination;
using Fliq.Contracts.MatchedProfile;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.MatchedList
{
    public record GetMatchRequestListCommand(int UserId) : IRequest<ErrorOr<List<MatchRequestDto>>>;


    public class CreateMatchListCommandHandler : IRequestHandler<GetMatchRequestListCommand, ErrorOr<List<MatchRequestDto>>>
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

        public async Task<ErrorOr<List<MatchRequestDto>>> Handle(GetMatchRequestListCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var pagination = new MatchListPagination();
            var filteredValue = await _matchProfileRepository.GetMatchListById(command.UserId, pagination);
            var result = filteredValue.ToList();

            return result;
        }
    }
}
