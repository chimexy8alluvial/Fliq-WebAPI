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
    }
}