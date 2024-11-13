using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Common.Pagination;
using Fliq.Contracts.MatchedProfile;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.MatchedList
{
    public record GetMatchRequestListCommand(int UserId, PaginationRequest PaginationRequest = default!) : IRequest<ErrorOr<List<MatchRequestDto>>>;


    public class CreateMatchListCommandHandler : IRequestHandler<GetMatchRequestListCommand, ErrorOr<List<MatchRequestDto>>>
    {
        private readonly IMatchProfileRepository _matchProfileRepository;

        public CreateMatchListCommandHandler(IMatchProfileRepository matchProfileRepository)
        {
 
            _matchProfileRepository = matchProfileRepository;
        }

        public async Task<ErrorOr<List<MatchRequestDto>>> Handle(GetMatchRequestListCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var filteredValue = await _matchProfileRepository.GetMatchListById(command);
            var result = filteredValue.ToList();

            return result;
        }
    }
}
