using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Commands.CreateStake;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Games;
using Moq;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class CreateStakeCommandHandlerTests
    {
        private Mock<IStakeRepository> _mockStakeRepository;
        private Mock<IWalletRepository> _mockWalletRepository;
        private Mock<ILoggerManager> _mockLogger;
        private Mock<IGamesRepository> _mockGameRepository;
        private CreateStakeCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockStakeRepository = new Mock<IStakeRepository>();
            _mockWalletRepository = new Mock<IWalletRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _mockGameRepository = new Mock<IGamesRepository>();

            _handler = new CreateStakeCommandHandler(
                _mockStakeRepository.Object,
                _mockWalletRepository.Object,
                _mockLogger.Object,
                _mockGameRepository.Object
            );
        }

        [TestMethod]
        public async Task Handle_InsufficientBalance_ReturnsError()
        {
            // Arrange
            var command = new CreateStakeCommand(1, 101, 102, 50m);

            _mockWalletRepository
                .Setup(repo => repo.GetWalletByUserId(command.RequesterId))
                .Returns(new Wallet { UserId = 101, Balance = 30m });
            _mockGameRepository.Setup(repo => repo.GetGameSessionById(command.GameSessionId)).Returns(new GameSession());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Wallet.InsufficientBalance, result.FirstError);
            _mockLogger.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Insufficient balance"))), Times.Once);
            _mockStakeRepository.Verify(repo => repo.Add(It.IsAny<Stake>()), Times.Never);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_CreatesStake()
        {
            // Arrange
            var command = new CreateStakeCommand(1, 101, 102, 50m);

            _mockWalletRepository
                .Setup(repo => repo.GetWalletByUserId(command.RequesterId))
                .Returns(new Wallet { UserId = 101, Balance = 100m });

            _mockWalletRepository
                .Setup(repo => repo.UpdateWallet(It.IsAny<Wallet>()));

            _mockGameRepository.Setup(repo => repo.GetGameSessionById(command.GameSessionId)).Returns(new GameSession());

            _mockStakeRepository
                .Setup(repo => repo.Add(It.IsAny<Stake>()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(command.GameSessionId, result.Value.GameSessionId);
            Assert.AreEqual(command.RequesterId, result.Value.RequesterId);
            Assert.AreEqual(command.RecipientId, result.Value.RecipientId);
            Assert.AreEqual(command.Amount, result.Value.Amount);

            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Creating stake"))), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Stake created"))), Times.Once);
            _mockWalletRepository.Verify(repo => repo.UpdateWallet(It.Is<Wallet>(w => w.UserId == command.RequesterId && w.Balance == 50m)), Times.Once);
            _mockStakeRepository.Verify(repo => repo.Add(It.IsAny<Stake>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_RequesterWalletNotFound_ReturnsError()
        {
            // Arrange
            var command = new CreateStakeCommand(1, 101, 102, 50m);

            _mockWalletRepository
                .Setup(repo => repo.GetWalletByUserId(command.RequesterId))
                .Returns((Wallet)null);
            _mockGameRepository.Setup(repo => repo.GetGameSessionById(command.GameSessionId)).Returns(new GameSession());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Wallet.InsufficientBalance, result.FirstError);
            _mockLogger.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Insufficient balance"))), Times.Once);
            _mockStakeRepository.Verify(repo => repo.Add(It.IsAny<Stake>()), Times.Never);
        }
    }
}