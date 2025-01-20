using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Commands.SendGameRequest;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Games;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class SendGameRequestCommandHandlerTests
    {
        private Mock<IGamesRepository> _mockGamesRepository;
        private Mock<ILoggerManager> _mockLogger;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IHubContext<GameHub>> _mockHub;

        private SendGameRequestCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockHub = new Mock<IHubContext<GameHub>>();
            _handler = new SendGameRequestCommandHandler(
                _mockGamesRepository.Object,
                _mockLogger.Object,
                _mockUserRepository.Object,
                _mockHub.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_SuccessfullySendsGameRequest()
        {
            // Arrange
            var recipient = new User { Id = 2, FirstName = "Recipient" };
            _mockUserRepository.Setup(repo => repo.GetUserById(2)).Returns(recipient);

            var command = new SendGameRequestCommand(GameId: 1, RequesterId: 1, RecipientId: 2);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(command.GameId, result.Value.GameRequest.GameId);
            Assert.AreEqual(command.RequesterId, result.Value.GameRequest.RequesterId);
            Assert.AreEqual(command.RecipientId, result.Value.GameRequest.RecipientId);

            _mockGamesRepository.Verify(repo => repo.AddGameRequest(It.IsAny<GameRequest>()), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Game request sent"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_RecipientNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns((User)null);

            var command = new SendGameRequestCommand(GameId: 1, RequesterId: 1, RecipientId: 999);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound.Code, result.FirstError.Code);

            _mockGamesRepository.Verify(repo => repo.AddGameRequest(It.IsAny<GameRequest>()), Times.Never);
            _mockLogger.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("not found"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_LogsInfoMessages()
        {
            // Arrange
            var recipient = new User { Id = 2, FirstName = "Recipient" };
            _mockUserRepository.Setup(repo => repo.GetUserById(2)).Returns(recipient);

            var command = new SendGameRequestCommand(GameId: 1, RequesterId: 1, RecipientId: 2);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Sending game request"))), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Game request sent"))), Times.Once);
        }
    }
}