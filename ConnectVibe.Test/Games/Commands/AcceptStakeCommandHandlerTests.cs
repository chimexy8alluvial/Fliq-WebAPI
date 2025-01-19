using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Games.Commands.AcceptStake;
using Moq;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Entities;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class AcceptStakeCommandHandlerTests
    {
        private Mock<IStakeRepository> _mockStakeRepository;
        private Mock<IWalletRepository> _mockWalletRepository;
        private AcceptStakeCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockStakeRepository = new Mock<IStakeRepository>();
            _mockWalletRepository = new Mock<IWalletRepository>();

            _handler = new AcceptStakeCommandHandler(
                _mockStakeRepository.Object,
                _mockWalletRepository.Object
            );
        }

        [TestMethod]
        public async Task Handle_StakeNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var command = new AcceptStakeCommand(1, 100);
            _mockStakeRepository.Setup(repo => repo.GetStakeById(It.IsAny<int>())).Returns((Stake)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Stake.NotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_InvalidRecipient_ReturnsInvalidRecipientError()
        {
            // Arrange
            var command = new AcceptStakeCommand(1, 100);
            var stake = new Stake { Id = 1, RecipientId = 200, IsAccepted = false };
            _mockStakeRepository.Setup(repo => repo.GetStakeById(It.IsAny<int>())).Returns(stake);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Stake.InvalidRecipient, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_AlreadyAccepted_ReturnsAlreadyAcceptedError()
        {
            // Arrange
            var command = new AcceptStakeCommand(1, 100);
            var stake = new Stake { Id = 1, RecipientId = 100, IsAccepted = true };
            _mockStakeRepository.Setup(repo => repo.GetStakeById(It.IsAny<int>())).Returns(stake);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Stake.AlreadyAccepted, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_AcceptsStakeAndDeductsBalance()
        {
            // Arrange
            var command = new AcceptStakeCommand(1, 100);
            var stake = new Stake { Id = 1, RecipientId = 100, Amount = 50, IsAccepted = false };
            var wallet = new Wallet { UserId = 100, Balance = 100 };

            _mockStakeRepository.Setup(repo => repo.GetStakeById(It.IsAny<int>())).Returns(stake);
            _mockWalletRepository.Setup(repo => repo.GetWalletByUserId(It.IsAny<int>())).Returns(wallet);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsTrue(result.Value.IsAccepted);

            _mockWalletRepository.Verify(repo => repo.UpdateWallet(wallet), Times.Once);
            _mockStakeRepository.Verify(repo => repo.UpdateStake(It.Is<Stake>(s => s.IsAccepted)), Times.Once);
        }
    }
}