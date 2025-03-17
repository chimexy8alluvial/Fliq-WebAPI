using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Games.Commands.CreateQuestion;
using Fliq.Domain.Entities.Games;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class CreateQuestionCommandHandlerTests
    {
        private Mock<IGamesRepository>? _mockGamesRepository;
        private Mock<ILogger<CreateQuestionCommandHandler>>? _mockLogger;
        private CreateQuestionCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILogger<CreateQuestionCommandHandler>>();

            _handler = new CreateQuestionCommandHandler(
                _mockGamesRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_CreatesQuestionSuccessfully()
        {
            // Arrange
            var command = new CreateQuestionCommand(
                GameId: 1,
                Text: "What is the capital of France?",
                Options: new List<string> { "Paris", "London", "Berlin", "Madrid" },
                CorrectAnswer: "Paris"
            );

            var game = new Game { Id = 1, Name = "Trivia Game" };

            _mockGamesRepository?
                .Setup(repo => repo.GetGameById(command.GameId))
                .Returns(game);

            GameQuestion capturedQuestion = null;
            _mockGamesRepository?
                .Setup(repo => repo.AddQuestion(It.IsAny<GameQuestion>()))
                .Callback<GameQuestion>(q => capturedQuestion = q);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(command.Text, result.Value.QuestionText);
            Assert.AreEqual(command.Options.Count, result.Value.Options.Count);

            Assert.IsNotNull(capturedQuestion);
            Assert.AreEqual(command.GameId, capturedQuestion.GameId);
            Assert.AreEqual(command.Text, capturedQuestion.QuestionText);
            Assert.AreEqual(command.CorrectAnswer, capturedQuestion.CorrectAnswer);
            Assert.AreEqual(command.Options.Count, capturedQuestion?.Options?.Count);

            _mockGamesRepository?.Verify(repo => repo.AddQuestion(It.IsAny<GameQuestion>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_GameNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var command = new CreateQuestionCommand(
                GameId: 99,
                Text: "What is the capital of France?",
                Options: new List<string> { "Paris", "London", "Berlin", "Madrid" },
                CorrectAnswer: "Paris"
            );

            _mockGamesRepository?
                .Setup(repo => repo.GetGameById(command.GameId))
                .Returns((Game?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Game.NotFound", result.FirstError.Code);

            _mockGamesRepository?.Verify(repo => repo.AddQuestion(It.IsAny<GameQuestion>()), Times.Never);
        }
    }
}