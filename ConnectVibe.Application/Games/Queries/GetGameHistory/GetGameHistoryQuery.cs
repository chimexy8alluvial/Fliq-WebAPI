using MediatR;
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;

namespace Fliq.Application.Games.Queries.GetGameHistory
{
    public record GetGameHistoryQuery(int Player1Id, int Player2Id)
        : IRequest<ErrorOr<List<GetGameHistoryResult>>>;

    public record GetGameHistoryResult(
        int HistoryId,
        string GameName,
        DateTime StartTime,
        DateTime? EndTime,
        int Player1Score,
        int Player2Score
    );

    public class GetGameHistoryQueryHandler
        : IRequestHandler<GetGameHistoryQuery, ErrorOr<List<GetGameHistoryResult>>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _logger;

        public GetGameHistoryQueryHandler(
            IGamesRepository gamesRepository,
            ILoggerManager logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<GetGameHistoryResult>>> Handle(
            GetGameHistoryQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Fetching game history between players {request.Player1Id} and {request.Player2Id}");
            await Task.CompletedTask;

            // Call the stored procedure
            var results = _gamesRepository.GetGameHistoryByTwoPlayers(request.Player1Id, request.Player2Id);

            if (results == null || !results.Any())
            {
                _logger.LogError("No game history found for the given players.");
                return Error.NotFound("GameHistory.NotFound", "No game history found for the given players.");
            }

            return results.Select(history => new GetGameHistoryResult(
                history.HistoryId,
                history.GameName,
                history.StartTime,
                history.EndTime,
                history.Player1Score,
                history.Player2Score
            )).ToList();
        }
    }
}