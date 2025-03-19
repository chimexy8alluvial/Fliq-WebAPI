using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Commands.RejectStake;
using Fliq.Domain.Entities.Games;
using Moq;
using Fliq.Domain.Common.Errors;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class RejectStakeCommandHandlerTests
    {
        private Mock<IStakeRepository>? _mockStakeRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private RejectStakeCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockStakeRepository = new Mock<IStakeRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new RejectStakeCommandHandler(_mockStakeRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_StakeNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var command = new RejectStakeCommand(1, 101);
            _mockStakeRepository?.Setup(repo => repo.GetStakeById(It.IsAny<int>())).Returns((Stake)null);

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
            var command = new RejectStakeCommand(1, 101);
            var stake = new Stake { Id = 1, RecipientId = 102 };
            _mockStakeRepository?.Setup(repo => repo.GetStakeById(It.IsAny<int>())).Returns(stake);

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
            var command = new RejectStakeCommand(1, 101);
            var stake = new Stake { Id = 1, RecipientId = 101, IsAccepted = true };
            _mockStakeRepository?.Setup(repo => repo.GetStakeById(It.IsAny<int>())).Returns(stake);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Stake.AlreadyAccepted, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidRejectRequest_UpdatesStake()
        {
            // Arrange
            var command = new RejectStakeCommand(1, 101);
            var stake = new Stake { Id = 1, RecipientId = 101, IsAccepted = false };
            _mockStakeRepository?.Setup(repo => repo.GetStakeById(It.IsAny<int>())).Returns(stake);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(stake, result.Value);
            _mockStakeRepository?.Verify(repo => repo.UpdateStake(It.IsAny<Stake>()), Times.Once);
        }
    }
}