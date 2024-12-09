using Fliq.Application.Games.Queries.GetGameHistory;
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

        void AddGameRequest(GameRequest gameRequest);

        void UpdateGameRequest(GameRequest gameRequest);

        GameRequest? GetGameRequestById(int id);

        void AddQuestion(GameQuestion question);

        GameQuestion? GetQuestionById(int id);

        List<GameQuestion> GetQuestionsByGameId(int gameId, int pageNumber, int pageSize);

        List<GetGameHistoryResult> GetGameHistoryByTwoPlayers(int player1Id, int player2Id);
    }
}