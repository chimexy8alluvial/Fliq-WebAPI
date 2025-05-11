using Fliq.Application.Games.Common;
using Fliq.Contracts.Games;
using Fliq.Domain.Entities.Games;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IGamesRepository
    {
        Game? GetGameById(int id);

        void AddGame(Game game);

        void UpdateGame(Game game);

        IEnumerable<Game> GetAllGames();

        void CreateGameSession(GameSession gameSession);

        void UpdateGameSession(GameSession gameSession);

        GameSession? GetGameSessionById(int id);
        Task<int> GetStakeCountByUserId(int userId);         
        void AddGameRequest(GameRequest gameRequest);

        void UpdateGameRequest(GameRequest gameRequest);

        GameRequest? GetGameRequestById(int id);

        void AddQuestion(GameQuestion question);

        GameQuestion? GetQuestionById(int id);

        List<GameQuestion> GetQuestionsByGameId(int gameId, int pageNumber = 1, int pageSize = 10);

        List<GetGameHistoryResult> GetGameHistoryByTwoPlayers(int player1Id, int player2Id);
        Task<int> GetActiveGamesCountAsync();
        Task<int> GetGamersCountAsync();
        Task<int> GetTotalGamesPlayedCountAsync();
        Task<int> GetGamesIssuesReportedCountAsync();
        Task<(List<GamesListItem> List, int totalCount)> GetAllGamesListAsync(int page, int pageSize, DateTime? datePlayedFrom, DateTime? datePlayedTo, int? status);

    }
}