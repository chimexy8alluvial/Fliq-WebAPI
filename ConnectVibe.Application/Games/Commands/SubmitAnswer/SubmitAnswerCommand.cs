using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Domain.Enums;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.Games.Commands.SubmitAnswer
{
    public record SubmitAnswerCommand(int SessionId, int PlayerId, string Answer)
       : IRequest<ErrorOr<SubmitAnswerResult>>;

    public class SubmitAnswerCommandHandler : IRequestHandler<SubmitAnswerCommand, ErrorOr<SubmitAnswerResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _loggerManager;

        public SubmitAnswerCommandHandler(IGamesRepository gamesRepository, ILoggerManager loggerManager)
        {
            _gamesRepository = gamesRepository;
            _loggerManager = loggerManager;
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

            var currentQuestion = session.Questions.FirstOrDefault(q => q.CorrectAnswer == null);
            if (currentQuestion == null)
            {
                _loggerManager.LogError($"No active question in session {request.SessionId}");
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

            _gamesRepository.UpdateGameSession(session);

            return new SubmitAnswerResult(isCorrect, session.Player1Score, session.Player2Score);
        }
    }
}