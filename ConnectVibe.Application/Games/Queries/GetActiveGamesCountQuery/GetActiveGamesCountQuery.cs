

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Games.Queries.GetActiveGamesCountQuery
{
    public class GetActiveGamesCountQuery
    {
        public record ActiveGamesCountQuery() : IRequest<ErrorOr<UserCountResult>>;

        public class ActiveGamesCountQueryHandler : IRequestHandler<ActiveGamesCountQuery, ErrorOr<UserCountResult>>
        {
            private readonly ILoggerManager _logger;
            private readonly IGamesRepository _gamesRepository;

            public ActiveGamesCountQueryHandler(ILoggerManager logger, IGamesRepository gamesRepository)
            {
                _logger = logger;
                _gamesRepository = gamesRepository;
            }
            public async Task<ErrorOr<UserCountResult>> Handle(ActiveGamesCountQuery query, CancellationToken cancellationToken)
            {
                _logger.LogInfo("Fetching all active games count ...");
                var count = await _gamesRepository.GetActiveGamesCountAsync();
                _logger.LogInfo($"All active games count: {count}");

                return new UserCountResult(count);
            }
        }

    }
}
