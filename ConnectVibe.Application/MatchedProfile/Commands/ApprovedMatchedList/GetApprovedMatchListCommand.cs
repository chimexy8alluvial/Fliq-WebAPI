using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Common.Pagination;
using Fliq.Contracts.MatchedProfile;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.ApprovedMatchedList
{
    public record GetApprovedMatchListCommand(int UserId, PaginationRequest PaginationRequest = default!) : IRequest<ErrorOr<List<MatchRequestDto>>>;

    public class GetApprovedMatchListCommandHandler : IRequestHandler<GetApprovedMatchListCommand, ErrorOr<List<MatchRequestDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IUserRepository _userRepository;
        private readonly IMatchProfileRepository _matchProfileRepository;
        public GetApprovedMatchListCommandHandler(IMapper mapper, IImageService imageService, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository)
        {
            _mapper = mapper;
            _imageService = imageService;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;
        }
        public async Task<ErrorOr<List<MatchRequestDto>>> Handle(GetApprovedMatchListCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var filteredValue = await _matchProfileRepository.GetApproveMatchListById(command);
            var result = filteredValue.ToList();

            return result;
        }
    }
}

