using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Pagination;
using Fliq.Contracts.MatchedProfile;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.MatchedList
{
    public record GetMatchRequestListCommand(int UserId) : IRequest<ErrorOr<List<MatchRequestDto>>>;


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

            var pagination = new MatchListPagination();
            var filteredValue = await _matchProfileRepository.GetMatchListById(command.UserId, pagination);
            var result = filteredValue.ToList();

            return result;
        }
    }
}
