using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Games;

namespace Fliq.Infrastructure.Persistence.Repositories.Adapters
{
    public class GamesRepositoryAdapter : IGenericRepository<Game>
    {
        private readonly IGamesRepository _gamesRepository;

        public GamesRepositoryAdapter(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }

        public  async Task<Game?> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return  _gamesRepository.GetGameById(id);
        }

        public async Task UpdateAsync(Game game)
        {
            await Task.CompletedTask;
             _gamesRepository.UpdateGame(game);
        }
    }
}
