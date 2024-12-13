using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using MediatR;

namespace Fliq.Application.Games.Queries.GetGames
{
    public record GetAllGamesQuery() : IRequest<List<GetGameResult>>;

    public class GetAllGamesQueryHandler : IRequestHandler<GetAllGamesQuery, List<GetGameResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _logger;

        public GetAllGamesQueryHandler(IGamesRepository gamesRepository, ILoggerManager logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }

        public async Task<List<GetGameResult>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Getting all games");
            await Task.CompletedTask;

            var games = _gamesRepository.GetAllGames();

            _logger.LogInfo($"Got {games.Count()} games");
            return games.Select(game => new GetGameResult(game)).ToList();
        }
    }
}