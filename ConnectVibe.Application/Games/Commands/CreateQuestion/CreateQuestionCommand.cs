using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Games.Common;
using Fliq.Domain.Entities.Games;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fliq.Application.Games.Commands.CreateQuestion
{
    public record CreateQuestionCommand(
        int GameId,
        string Text,
        List<string> Options,
        int CorrectOptionIndex
    ) : IRequest<ErrorOr<GetQuestionResult>>;

    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, ErrorOr<GetQuestionResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILogger<CreateQuestionCommandHandler> _logger;

        public CreateQuestionCommandHandler(
            IGamesRepository gamesRepository,
            ILogger<CreateQuestionCommandHandler> logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<GetQuestionResult>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Creating question for game {request.GameId}");

            // Ensure the game exists
            var game = _gamesRepository.GetGameById(request.GameId);
            if (game == null)
            {
                _logger.LogError($"Game with ID {request.GameId} not found.");
                return Error.NotFound("Game.NotFound", "The specified game does not exist.");
            }

            // Validate CorrectOptionIndex
            if (request.CorrectOptionIndex < 0 || request.CorrectOptionIndex >= request.Options.Count)
            {
                _logger.LogError($"Invalid correct option index {request.CorrectOptionIndex} for question.");
                return Error.Validation("Question.InvalidCorrectOption", "The correct option index is out of range.");
            }

            // Create the question
            var question = new GameQuestion
            {
                GameId = request.GameId,
                QuestionText = request.Text,
                Options = request.Options,
                CorrectAnswer = request.Options.ElementAt(request.CorrectOptionIndex)
            };

            _gamesRepository.AddQuestion(question);

            _logger.LogInformation($"Question created with ID {question.Id}");

            return new GetQuestionResult(
                question.Id,
                question.GameId,
                question.QuestionText,
                question.Options,
                question.CorrectAnswer
            );
        }
    }
}