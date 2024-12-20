using ErrorOr;
using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Domain.Entities.Games;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.Games.Commands.CreateGameSession
{
    public record CreateGameSessionCommand(int GameId, int Player1Id, int Player2Id) : IRequest<ErrorOr<GetGameSessionResult>>;

    public class CreateGameSessionCommandHandler : IRequestHandler<CreateGameSessionCommand, ErrorOr<GetGameSessionResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _logger;
        private readonly IHubContext<GameHub> _hubContext;

        public CreateGameSessionCommandHandler(IGamesRepository gamesRepository, ILoggerManager logger, IHubContext<GameHub> hubContext)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task<ErrorOr<GetGameSessionResult>> Handle(CreateGameSessionCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Creating game session for GameId: {request.GameId}");

            var session = new GameSession
            {
                GameId = request.GameId,
                Player1Id = request.Player1Id,
                Player2Id = request.Player2Id,
                StartTime = DateTime.UtcNow
            };

            _gamesRepository.CreateGameSession(session);

            await _hubContext.Clients.All.SendAsync("GameSessionCreated", session);

            _logger.LogInfo($"Game session created: {session.Id}");
            return new GetGameSessionResult(session);
        }
    }
}