using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Games.Queries.GetSession
{
    public record GetGameSessionQuery(int SessionId) : IRequest<ErrorOr<GetGameSessionResult>>;

    public class GetGameSessionQueryHandler : IRequestHandler<GetGameSessionQuery, ErrorOr<GetGameSessionResult>>
    {
        private readonly IGamesRepository _sessionsRepository;
        private readonly ILoggerManager _logger;

        public GetGameSessionQueryHandler(IGamesRepository sessionsRepository, ILoggerManager logger)
        {
            _sessionsRepository = sessionsRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<GetGameSessionResult>> Handle(GetGameSessionQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Getting game session with id: {request.SessionId}");
            await Task.CompletedTask;

            var session = _sessionsRepository.GetGameSessionById(request.SessionId);

            if (session is null)
            {
                _logger.LogError($"Game session with id: {request.SessionId} not found");
                return Errors.Games.GameNotFound;
            }

            var questions = _sessionsRepository.GetQuestionsByGameId(session.GameId, int.MaxValue, int.MaxValue);

            if (questions is null)
            {
                _logger.LogError($"Questions for game with id: {session.GameId} not found");
                return Errors.Games.GameNotFound;
            }
            session.Questions = questions;
            _logger.LogInfo($"Game session with id: {request.SessionId} found");
            return new GetGameSessionResult(session);
        }
    }
}