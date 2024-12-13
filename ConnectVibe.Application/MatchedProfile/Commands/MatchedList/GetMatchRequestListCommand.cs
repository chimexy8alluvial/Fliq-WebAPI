using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.MatchedList
{
    public record GetMatchRequestListCommand(int UserId, PaginationRequest PaginationRequest = default!, MatchRequestStatus? MatchRequestStatus = null) : IRequest<ErrorOr<List<MatchRequestDto>>>;

    public class CreateMatchListCommandHandler : IRequestHandler<GetMatchRequestListCommand, ErrorOr<List<MatchRequestDto>>>
    {
        private readonly IMatchProfileRepository _matchProfileRepository;
        private readonly ILoggerManager _logger;

        public CreateMatchListCommandHandler(IMatchProfileRepository matchProfileRepository, ILoggerManager logger)
        {
            _matchProfileRepository = matchProfileRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<MatchRequestDto>>> Handle(GetMatchRequestListCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo("GetMatchRequestListCommandHandler is called");
            await Task.CompletedTask;

            var request = new GetMatchListRequest
            {
                UserId = command.UserId,
                PaginationRequest = command.PaginationRequest,
                MatchRequestStatus = command.MatchRequestStatus,
            };

            var filteredValue = await _matchProfileRepository.GetMatchListById(request);
            var result = filteredValue.ToList();

            _logger.LogInfo("GetMatchRequestListCommandHandler is completed");
            return result;
        }
    }
}