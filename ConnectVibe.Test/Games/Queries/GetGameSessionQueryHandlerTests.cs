using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Queries.GetSession;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Games;
using Moq;

namespace Fliq.Test.Games.Queries.GetSession
{
    [TestClass]
    public class GetGameSessionQueryHandlerTests
    {
        private Mock<IGamesRepository>? _mockSessionsRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetGameSessionQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSessionsRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new GetGameSessionQueryHandler(
                _mockSessionsRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_SessionExists_ReturnsSessionSuccessfully()
        {
            // Arrange
            var sessionId = 1;
            var gameId = 100;
            var session = new GameSession
            {
                Id = sessionId,
                GameId = gameId,
                Player1Id = 1,
                Player2Id = 2
            };

            _mockSessionsRepository?.Setup(repo => repo.GetGameSessionById(sessionId)).Returns(session);

            var query = new GetGameSessionQuery(sessionId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(sessionId, result.Value.GameSession.Id);
            Assert.AreEqual(gameId, result.Value.GameSession.GameId);

            _mockSessionsRepository?.Verify(repo => repo.GetGameSessionById(sessionId), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Getting game session with id: {sessionId}"))), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Game session with id: {sessionId} found"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_SessionDoesNotExist_ReturnsError()
        {
            // Arrange
            var sessionId = 1;
            _mockSessionsRepository?.Setup(repo => repo.GetGameSessionById(sessionId)).Returns((GameSession)null);

            var query = new GetGameSessionQuery(sessionId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Games.GameNotFound, result.FirstError);

            _mockSessionsRepository?.Verify(repo => repo.GetGameSessionById(sessionId), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Getting game session with id: {sessionId}"))), Times.Once);
            _mockLogger?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains($"Game session with id: {sessionId} not found"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_LogsCorrectMessages_WhenSessionExists()
        {
            // Arrange
            var sessionId = 1;
            var gameId = 100;
            var session = new GameSession
            {
                Id = sessionId,
                GameId = gameId,
                Player1Id = 1,
                Player2Id = 2
            };
            var questions = new List<GameQuestion>
            {
                new GameQuestion { Id = 1, GameId = gameId, QuestionText = "Q1", CorrectAnswer = "A" }
            };

            _mockSessionsRepository?.Setup(repo => repo.GetGameSessionById(sessionId)).Returns(session);
            _mockSessionsRepository?.Setup(repo => repo.GetQuestionsByGameId(gameId, int.MaxValue, int.MaxValue)).Returns(questions);

            var query = new GetGameSessionQuery(sessionId);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Getting game session with id: {sessionId}"))), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Game session with id: {sessionId} found"))), Times.Once);
        }
    }
}