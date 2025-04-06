

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Games.Queries.GetActiveGamesCountQuery
{
    public record GetActiveGamesCountQuery() : IRequest<ErrorOr<UserCountResult>>;

        public class GetActiveGamesCountQueryHandler : IRequestHandler<GetActiveGamesCountQuery, ErrorOr<UserCountResult>>
        {
            private readonly ILoggerManager _logger;
            private readonly IGamesRepository _gamesRepository;

            public GetActiveGamesCountQueryHandler(ILoggerManager logger, IGamesRepository gamesRepository)
            {
                _logger = logger;
                _gamesRepository = gamesRepository;
            }
            public async Task<ErrorOr<UserCountResult>> Handle(GetActiveGamesCountQuery query, CancellationToken cancellationToken)
            {
                _logger.LogInfo("Fetching all active games count ...");
                var count = await _gamesRepository.GetActiveGamesCountAsync();
                _logger.LogInfo($"All active games count: {count}");

                return new UserCountResult(count);
            }
        }

}

