using ErrorOr;
using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.Games.Commands.SubmitAnswer
{
    public record SubmitAnswerCommand(int SessionId, int Player1Score, int Player2Score)
       : IRequest<ErrorOr<SubmitAnswerResult>>;

    public class SubmitAnswerCommandHandler : IRequestHandler<SubmitAnswerCommand, ErrorOr<SubmitAnswerResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IHubContext<GameHub> _hubContext;

        public SubmitAnswerCommandHandler(IGamesRepository gamesRepository, ILoggerManager loggerManager, IHubContext<GameHub> hubContext)
        {
            _gamesRepository = gamesRepository;
            _loggerManager = loggerManager;
            _hubContext = hubContext;
        }

        public async Task<ErrorOr<SubmitAnswerResult>> Handle(SubmitAnswerCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var session = _gamesRepository.GetGameSessionById(request.SessionId);

            if (session == null || session.Status != GameStatus.InProgress)
            {
                _loggerManager.LogError($"Session {request.SessionId} not found");
                return Errors.Games.GameNotFound;
            }

            session.Player1Score = request.Player1Score;
            session.Player2Score = request.Player2Score;
            session.Status = GameStatus.Done;

            _gamesRepository.UpdateGameSession(session);

            // Notify players via SignalR
            await _hubContext.Clients.Group(session.Id.ToString()).SendAsync("GameEnded", session);

            return new SubmitAnswerResult(session);
        }
    }
}