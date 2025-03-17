using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Commands.AcceptStake;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Games;
using Moq;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class AcceptStakeCommandHandlerTests
    {
        private Mock<IStakeRepository>? _mockStakeRepository;
        private Mock<IWalletRepository>? _mockWalletRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private AcceptStakeCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockStakeRepository = new Mock<IStakeRepository>();
            _mockWalletRepository = new Mock<IWalletRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new AcceptStakeCommandHandler(
                _mockStakeRepository.Object,
                _mockWalletRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_StakeNotFound_ReturnsError()
        {
            // Arrange
            var command = new AcceptStakeCommand(1, 101);

            _mockStakeRepository?
                .Setup(repo => repo.GetStakeById(command.StakeId))
                .Returns((Stake?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Stake.NotFound, result.FirstError);
            _mockLogger?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Stake with Id: 1 not found"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_InvalidRecipient_ReturnsError()
        {
            // Arrange
            var stake = new Stake { Id = 1, RecipientId = 102, Amount = 50m };
            var command = new AcceptStakeCommand(1, 101);

            _mockStakeRepository?
                .Setup(repo => repo.GetStakeById(command.StakeId))
                .Returns(stake);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Stake.InvalidRecipient, result.FirstError);
            _mockLogger?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Invalid recipient"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_AlreadyAccepted_ReturnsError()
        {
            // Arrange
            var stake = new Stake { Id = 1, RecipientId = 101, Amount = 50m, IsAccepted = true };
            var command = new AcceptStakeCommand(1, 101);

            _mockStakeRepository?
                .Setup(repo => repo.GetStakeById(command.StakeId))
                .Returns(stake);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Stake.AlreadyAccepted, result.FirstError);
            _mockLogger?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("already been accepted"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_InsufficientBalance_ReturnsError()
        {
            // Arrange
            var stake = new Stake { Id = 1, RecipientId = 101, Amount = 50m, IsAccepted = false };
            var command = new AcceptStakeCommand(1, 101);

            _mockStakeRepository?
                .Setup(repo => repo.GetStakeById(command.StakeId))
                .Returns(stake);

            _mockWalletRepository?
                .Setup(repo => repo.GetWalletByUserId(command.UserId))
                .Returns(new Wallet { Id = 1, UserId = 101, Balance = 30m });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Wallet.InsufficientBalance, result.FirstError);
            _mockLogger?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Insufficient balance"))), Times.Once);
            _mockWalletRepository?.Verify(repo => repo.AddWalletHistory(It.Is<WalletHistory>(history =>
                history.ActivityType == WalletActivityType.Withdrawal &&
                history.Amount == 50m &&
                history.TransactionStatus == WalletTransactionStatus.Failed
            )), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_AcceptsStake()
        {
            // Arrange
            var stake = new Stake { Id = 1, RecipientId = 101, Amount = 50m, IsAccepted = false };
            var command = new AcceptStakeCommand(1, 101);

            _mockStakeRepository?
                .Setup(repo => repo.GetStakeById(command.StakeId))
                .Returns(stake);

            var wallet = new Wallet { Id = 1, UserId = 101, Balance = 100m };

            _mockWalletRepository?
                .Setup(repo => repo.GetWalletByUserId(command.UserId))
                .Returns(wallet);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(stake, result.Value);
            _mockLogger?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Withdrawal of 50 for stake successful"))), Times.Once);
            _mockWalletRepository?.Verify(repo => repo.UpdateWallet(It.Is<Wallet>(w => w.Balance == 50m)), Times.Once);
            _mockWalletRepository?.Verify(repo => repo.AddWalletHistory(It.Is<WalletHistory>(history =>
                history.ActivityType == WalletActivityType.Withdrawal &&
                history.Amount == 50m &&
                history.TransactionStatus == WalletTransactionStatus.Success
            )), Times.Once);
            _mockStakeRepository?.Verify(repo => repo.UpdateStake(It.Is<Stake>(s => s.IsAccepted)), Times.Once);
        }
    }
}