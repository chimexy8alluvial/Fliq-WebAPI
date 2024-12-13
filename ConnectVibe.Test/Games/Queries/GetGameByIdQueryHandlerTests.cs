using Moq;
using Fliq.Application.Games.Queries.GetGame;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Common.Errors;

namespace Fliq.Test.Games.Queries.GetGame
{
    [TestClass]
    public class GetGameByIdQueryHandlerTests
    {
        private Mock<IGamesRepository> _mockGamesRepository;
        private Mock<ILoggerManager> _mockLogger;
        private GetGameByIdQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new GetGameByIdQueryHandler(
                _mockGamesRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_GameExists_ReturnsGameSuccessfully()
        {
            // Arrange
            var gameId = 1;
            var game = new Game { Id = gameId, Name = "Test Game" };
            _mockGamesRepository.Setup(repo => repo.GetGameById(gameId)).Returns(game);

            var query = new GetGameByIdQuery(gameId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(gameId, result.Value.Game.Id);
            Assert.AreEqual("Test Game", result.Value.Game.Name);

            _mockGamesRepository.Verify(repo => repo.GetGameById(gameId), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Getting game with id: {gameId}"))), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Got game with id: {gameId}"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_GameDoesNotExist_ReturnsError()
        {
            // Arrange
            var gameId = 1;
            _mockGamesRepository.Setup(repo => repo.GetGameById(gameId)).Returns((Game)null);

            var query = new GetGameByIdQuery(gameId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Games.GameNotFound, result.FirstError);

            _mockGamesRepository.Verify(repo => repo.GetGameById(gameId), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Getting game with id: {gameId}"))), Times.Once);
            _mockLogger.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains($"Game with id: {gameId} not found"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_LogsCorrectMessages()
        {
            // Arrange
            var gameId = 1;
            var game = new Game { Id = gameId, Name = "Test Game" };
            _mockGamesRepository.Setup(repo => repo.GetGameById(gameId)).Returns(game);

            var query = new GetGameByIdQuery(gameId);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Getting game with id: {gameId}"))), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Got game with id: {gameId}"))), Times.Once);
        }
    }
}