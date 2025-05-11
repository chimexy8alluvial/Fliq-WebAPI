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
                CorrectOptionIndex: 0 // Changed from CorrectAnswer to CorrectOptionIndex
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
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(command.Text, result.Value.QuestionText);
            Assert.AreEqual(command.Options.Count, result.Value.Options.Count);
            Assert.AreEqual(command.Options[command.CorrectOptionIndex], result.Value.CorrectAnswer);

            Assert.IsNotNull(capturedQuestion);
            Assert.AreEqual(command.GameId, capturedQuestion.GameId);
            Assert.AreEqual(command.Text, capturedQuestion.QuestionText);
            Assert.AreEqual(command.Options[command.CorrectOptionIndex], capturedQuestion.CorrectAnswer);
            Assert.AreEqual(command.Options.Count, capturedQuestion.Options.Count);

            _mockGamesRepository?.Verify(repo => repo.GetGameById(command.GameId), Times.Once());
            _mockGamesRepository?.Verify(repo => repo.AddQuestion(It.IsAny<GameQuestion>()), Times.Once());
        }

        [TestMethod]
        public async Task Handle_GameNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var command = new CreateQuestionCommand(
                GameId: 99,
                Text: "What is the capital of France?",
                Options: new List<string> { "Paris", "London", "Berlin", "Madrid" },
                CorrectOptionIndex: 0 // Changed from CorrectAnswer to CorrectOptionIndex
            );

            _mockGamesRepository?
                .Setup(repo => repo.GetGameById(command.GameId))
                .Returns((Game?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Game.NotFound", result.FirstError.Code);
            Assert.AreEqual("The specified game does not exist.", result.FirstError.Description);

            _mockGamesRepository?.Verify(repo => repo.GetGameById(command.GameId), Times.Once());
            _mockGamesRepository?.Verify(repo => repo.AddQuestion(It.IsAny<GameQuestion>()), Times.Never());
        }

        [TestMethod]
        public async Task Handle_InvalidCorrectOptionIndex_ReturnsValidationError()
        {
            // Arrange
            var command = new CreateQuestionCommand(
                GameId: 1,
                Text: "What is the capital of France?",
                Options: new List<string> { "Paris", "London", "Berlin", "Madrid" },
                CorrectOptionIndex: 4 // Invalid index
            );

            var game = new Game { Id = 1, Name = "Trivia Game" };

            _mockGamesRepository?
                .Setup(repo => repo.GetGameById(command.GameId))
                .Returns(game);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Question.InvalidCorrectOption", result.FirstError.Code);
            Assert.AreEqual("The correct option index is out of range.", result.FirstError.Description);

            _mockGamesRepository?.Verify(repo => repo.GetGameById(command.GameId), Times.Once());
            _mockGamesRepository?.Verify(repo => repo.AddQuestion(It.IsAny<GameQuestion>()), Times.Never());
        }
    }
}