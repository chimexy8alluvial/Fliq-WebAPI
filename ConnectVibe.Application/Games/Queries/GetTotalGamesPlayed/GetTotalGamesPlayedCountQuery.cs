using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Games.Queries.GetTotalGamesPlayed
{
    public record GetTotalGamesPlayedCountQuery() : IRequest<ErrorOr<CountResult>>;
        public class GetTotalGamesPlayedCountQueryHandler : IRequestHandler<GetTotalGamesPlayedCountQuery, ErrorOr<CountResult>>
        {
            private readonly ILoggerManager _logger;
            private readonly IGamesRepository _gamesRepository;
            public GetTotalGamesPlayedCountQueryHandler(ILoggerManager logger, IGamesRepository gamesRepository)
            {
                _logger = logger;
                _gamesRepository = gamesRepository;
            }

            public async Task<ErrorOr<CountResult>> Handle(GetTotalGamesPlayedCountQuery query, CancellationToken cancellationToken)
            {
                _logger.LogInfo("Fetching all total games played count ...");
                var count = await _gamesRepository.GetTotalGamesPlayedCountAsync();
                _logger.LogInfo($"All total games played count: {count}");

                return new CountResult(count);
            }
        }

}

