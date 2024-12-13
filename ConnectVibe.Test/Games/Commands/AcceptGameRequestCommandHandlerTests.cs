using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Commands.AcceptGameRequest;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class AcceptGameRequestCommandHandlerTests
    {
        private Mock<IGamesRepository> _mockGamesRepository;
        private Mock<ILoggerManager> _mockLogger;
        private AcceptGameRequestCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new AcceptGameRequestCommandHandler(
                _mockGamesRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_InvalidGameRequest_ReturnsInvalidGameStateError()
        {
            // Arrange
            var command = new AcceptGameRequestCommand(1, 101);
            _mockGamesRepository
                .Setup(repo => repo.GetGameRequestById(It.IsAny<int>()))
                .Returns((GameRequest)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Games.InvalidGameState, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_GameRequestNotPending_ReturnsInvalidGameStateError()
        {
            // Arrange
            var command = new AcceptGameRequestCommand(1, 101);
            var gameRequest = new GameRequest
            {
                Id = 1,
                Status = GameStatus.Rejected
            };

            _mockGamesRepository
                .Setup(repo => repo.GetGameRequestById(It.IsAny<int>()))
                .Returns(gameRequest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Games.InvalidGameState, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidGameRequest_UpdatesStatusAndCreatesGameSession()
        {
            // Arrange
            var command = new AcceptGameRequestCommand(1, 101);
            var gameRequest = new GameRequest
            {
                Id = 1,
                GameId = 10,
                RequesterId = 100,
                Status = GameStatus.Pending
            };

            _mockGamesRepository
                .Setup(repo => repo.GetGameRequestById(It.IsAny<int>()))
                .Returns(gameRequest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(GameStatus.InProgress, result.Value.GameSession.Status);
            Assert.AreEqual(gameRequest.RequesterId, result.Value.GameSession.Player1Id);
            Assert.AreEqual(command.UserId, result.Value.GameSession.Player2Id);

            _mockGamesRepository.Verify(repo => repo.UpdateGameRequest(It.Is<GameRequest>(gr => gr.Status == GameStatus.Accepted)), Times.Once);
            _mockGamesRepository.Verify(repo => repo.CreateGameSession(It.IsAny<GameSession>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_LogsActionsDuringExecution()
        {
            // Arrange
            var command = new AcceptGameRequestCommand(1, 101);
            var gameRequest = new GameRequest
            {
                Id = 1,
                GameId = 10,
                RequesterId = 100,
                Status = GameStatus.Pending
            };

            _mockGamesRepository
                .Setup(repo => repo.GetGameRequestById(It.IsAny<int>()))
                .Returns(gameRequest);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Accepting game request"))), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Accepted game request"))), Times.Once);
        }
    }
}