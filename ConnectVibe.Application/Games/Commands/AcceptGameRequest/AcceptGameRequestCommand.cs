using ErrorOr;
using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.Games.Commands.AcceptGameRequest
{
    public record AcceptGameRequestCommand(int RequestId, int UserId) : IRequest<ErrorOr<GetGameSessionResult>>;

    public class AcceptGameRequestCommandHandler : IRequestHandler<AcceptGameRequestCommand, ErrorOr<GetGameSessionResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _logger;
        private readonly IHubContext<GameHub> _hubContext;

        public AcceptGameRequestCommandHandler(IGamesRepository gamesRepository, ILoggerManager logger, IHubContext<GameHub> hubContext)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task<ErrorOr<GetGameSessionResult>> Handle(AcceptGameRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Accepting game request: {request.RequestId}");
            await Task.CompletedTask;
            var gameRequest = _gamesRepository.GetGameRequestById(request.RequestId);

            if (gameRequest == null || gameRequest.Status != GameStatus.Pending)
                return Errors.Games.InvalidGameState;

            gameRequest.Status = GameStatus.Accepted;
            _gamesRepository.UpdateGameRequest(gameRequest);

            var gameSession = new GameSession
            {
                GameId = gameRequest.GameId,
                Player1Id = gameRequest.RequesterId,
                Player2Id = request.UserId,
                CurrentTurnPlayerId = gameRequest.RequesterId,
                Status = GameStatus.InProgress
            };

            _gamesRepository.CreateGameSession(gameSession);

            await _hubContext.Clients
                .Group(gameSession.Id.ToString())
                .SendAsync("GameRequestAccepted", gameSession);

            _logger.LogInfo($"Accepted game request: {request.RequestId}");
            return new GetGameSessionResult(gameSession);
        }
    }
}