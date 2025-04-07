using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Commands.CreateGameSession;
using Fliq.Domain.Entities.Games;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class CreateGameSessionCommandHandlerTests
    {
        private Mock<IGamesRepository>? _mockGamesRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private Mock<IHubContext<GameHub>>? _mockHub;
        private CreateGameSessionCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _mockHub = new Mock<IHubContext<GameHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            _mockHub.Setup(h => h.Clients).Returns(mockClients.Object);
            _handler = new CreateGameSessionCommandHandler(
                _mockGamesRepository.Object,
                _mockLogger.Object,
                _mockHub.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_CreatesGameSessionSuccessfully()
        {
            // Arrange
            var command = new CreateGameSessionCommand(GameId: 1, Player1Id: 100, Player2Id: 200, GameDisconnectionResolutionOption.LastManWins);

            _mockGamesRepository?
                .Setup(repo => repo.CreateGameSession(It.IsAny<GameSession>()))
                .Callback<GameSession>(session => session.Id = 1); // Simulate assigning an ID

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.GameSession.Id);
            Assert.AreEqual(command.GameId, result.Value.GameSession.GameId);
            Assert.AreEqual(command.Player1Id, result.Value.GameSession.Player1Id);
            Assert.AreEqual(command.Player2Id, result.Value.GameSession.Player2Id);

            _mockGamesRepository?.Verify(repo => repo.CreateGameSession(It.IsAny<GameSession>()), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Creating game session"))), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Game session created"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_LogsInformationMessages()
        {
            // Arrange
            var command = new CreateGameSessionCommand(GameId: 1, Player1Id: 100, Player2Id: 200, GameDisconnectionResolutionOption.LastManWins);

            _mockGamesRepository?
                .Setup(repo => repo.CreateGameSession(It.IsAny<GameSession>()))
                .Callback<GameSession>(session => session.Id = 1);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Creating game session"))), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Game session created"))), Times.Once);
        }
    }
}