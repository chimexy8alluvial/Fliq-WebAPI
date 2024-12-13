using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Domain.Entities.Games;
using MediatR;
using ErrorOr;

namespace Fliq.Application.Games.Commands.CreateGame
{
    public record CreateGameCommand(string Name, string? Description, bool RequiresLevel, bool RequiresTheme, bool RequiresCategory) : IRequest<ErrorOr<GetGameResult>>;

    public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, ErrorOr<GetGameResult>>
    {
        private readonly IGamesRepository _gameRepository;
        private readonly ILoggerManager _logger;

        public CreateGameCommandHandler(IGamesRepository gameRepository, ILoggerManager logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<GetGameResult>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Creating game: {request.Name}");
            var game = new Game
            {
                Name = request.Name,
                Description = request.Description,
                RequiresLevel = request.RequiresLevel,
                RequiresTheme = request.RequiresTheme,
                RequiresCategory = request.RequiresCategory
            };

            _gameRepository.AddGame(game);

            _logger.LogInfo($"Game created: {game.Id}");
            return new GetGameResult(game);
        }
    }
}