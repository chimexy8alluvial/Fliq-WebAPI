using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Games;
using MediatR;

namespace Fliq.Application.Games.Queries.GetQuestions
{
    public record GetQuestionsQuery(int GameId, int pageSize = 10, int pageNumber = 1) : IRequest<ErrorOr<List<GameQuestion>>>;

    public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, ErrorOr<List<GameQuestion>>>
    {
        private readonly IGamesRepository _sessionsRepository;
        private readonly ILoggerManager _logger;

        public GetQuestionsQueryHandler(IGamesRepository sessionsRepository, ILoggerManager logger)
        {
            _sessionsRepository = sessionsRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<GameQuestion>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Getting game Questions for game  with id: {request.GameId}");
            await Task.CompletedTask;

            var questions = _sessionsRepository.GetQuestionsByGameId(request.GameId, request.pageNumber, request.pageSize);

            if (!questions.Any())
            {
                _logger.LogWarn($"No questions found for GameId: {request.GameId}");
                return Errors.Games.QuestionNotFound;
            }

            _logger.LogInfo($" Questions for Game with id: {request.GameId} found");
            return questions;
        }
    }
}