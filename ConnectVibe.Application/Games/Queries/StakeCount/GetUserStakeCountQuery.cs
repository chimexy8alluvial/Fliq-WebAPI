using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Games.Queries.StakeCount
{
    public record GetUserStakeCountQuery(int UserId) : IRequest<ErrorOr<CountResult>>;

    public class GetUserStakeCountQueryHandler : IRequestHandler<GetUserStakeCountQuery, ErrorOr<CountResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _logger;

        public GetUserStakeCountQueryHandler(IGamesRepository gamesRepository, ILoggerManager logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetUserStakeCountQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Fetching stake count for user with ID --> {request.UserId}");

            var totalStakesInUserGameSessions = await _gamesRepository.GetStakeCountByUserId(request.UserId);

            var actualStakeInUserGameSessions = totalStakesInUserGameSessions / 2;

            _logger.LogInfo($"User {request.UserId} Stake Count: {actualStakeInUserGameSessions}");

            return new CountResult(actualStakeInUserGameSessions);
       
        }
    }
}
