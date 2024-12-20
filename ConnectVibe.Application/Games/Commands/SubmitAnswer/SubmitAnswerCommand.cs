using ErrorOr;
using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Contracts.Games;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.Games.Commands.SubmitAnswer
{
    public record SubmitAnswerCommand(int SessionId, int PlayerId, string Answer)
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

            if (session.CurrentTurnPlayerId != request.PlayerId)
            {
                _loggerManager.LogError(($"Not your turn! {session.CurrentTurnPlayerId} vs {request.PlayerId}"));
                return Errors.Games.NotYourTurn;
            }

            var questions = _gamesRepository.GetQuestionsByGameId(session.GameId, int.MaxValue, int.MaxValue);

            var currentQuestion = questions[session.CurrentQuestionIndex];
            if (currentQuestion == null)
            {
                _loggerManager.LogError($"No active question in session {request.SessionId}");
                session.Status = GameStatus.Done;
                return Errors.Games.NoActiveQuestion;
            }

            var isCorrect = string.Equals(currentQuestion.CorrectAnswer, request.Answer, StringComparison.OrdinalIgnoreCase);

            if (request.PlayerId == session.Player1Id)
                session.Player1Score += isCorrect ? 1 : 0;
            else
                session.Player2Score += isCorrect ? 1 : 0;

            session.CurrentTurnPlayerId = session.CurrentTurnPlayerId == session.Player1Id
                ? session.Player2Id
                : session.Player1Id;
            session.CurrentQuestionIndex++;
            _gamesRepository.UpdateGameSession(session);

            var updatedGameState = new UpdatedGameState
            {
                SessionId = session.Id,
                Player1Score = session.Player1Score,
                Player2Score = session.Player2Score,
                CurrentTurnPlayerId = session.CurrentTurnPlayerId,
                CurrentQuestionIndex = session.CurrentQuestionIndex,
                IsGameDone = session.Status == GameStatus.Done
            };

            // Notify players via SignalR
            await _hubContext.Clients.Group(session.Id.ToString()).SendAsync("GameUpdated", updatedGameState);

            return new SubmitAnswerResult(isCorrect, session.Player1Score, session.Player2Score);
        }
    }
}