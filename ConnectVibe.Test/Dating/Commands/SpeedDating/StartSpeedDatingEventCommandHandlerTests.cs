using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands.SpeedDating;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using Moq;

namespace Fliq.Test.Dating.Commands.SpeedDating
{
    [TestClass]
    public class StartSpeedDatingEventCommandHandlerTests
    {
        private Mock<ISpeedDatingEventRepository> _mockSpeedDateRepository;
        private Mock<ISpeedDateParticipantRepository> _mockSpeedDateParticipantRepository;
        private Mock<ILoggerManager> _mockLoggerManager;
        private StartSpeedDatingEventCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSpeedDateRepository = new Mock<ISpeedDatingEventRepository>();
            _mockSpeedDateParticipantRepository = new Mock<ISpeedDateParticipantRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();

            _handler = new StartSpeedDatingEventCommandHandler(
                _mockSpeedDateRepository.Object,
                _mockSpeedDateParticipantRepository.Object,
                _mockLoggerManager.Object);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_StartsSpeedDatingEventSuccessfully()
        {
            // Arrange
            var command = new StartSpeedDatingEventCommand(1, 100);
            var speedDate = new SpeedDatingEvent { Id = 100, Status = DateStatus.Pending };
            var creator = new SpeedDatingParticipant { UserId = 1, SpeedDatingEventId = 100, IsCreator = true };

            _mockSpeedDateRepository.Setup(repo => repo.GetByIdAsync(100)).ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(100)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(DateStatus.Ongoing, speedDate.Status);
            _mockSpeedDateRepository.Verify(repo => repo.UpdateAsync(It.IsAny<SpeedDatingEvent>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_SpeedDatingEventNotFound_ReturnsError()
        {
            // Arrange
            var command = new StartSpeedDatingEventCommand(1, 200);
            _mockSpeedDateRepository.Setup(repo => repo.GetByIdAsync(200)).ReturnsAsync((SpeedDatingEvent)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("not found"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_UserNotCreator_ReturnsError()
        {
            // Arrange
            var command = new StartSpeedDatingEventCommand(2, 100);
            var speedDate = new SpeedDatingEvent { Id = 100, Status = DateStatus.Pending };
            var creator = new SpeedDatingParticipant { UserId = 1, SpeedDatingEventId = 100, IsCreator = true };

            _mockSpeedDateRepository.Setup(repo => repo.GetByIdAsync(100)).ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(100)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("not the creator"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_EventAlreadyStarted_ReturnsError()
        {
            // Arrange
            var command = new StartSpeedDatingEventCommand(1, 100);
            var speedDate = new SpeedDatingEvent { Id = 100, Status = DateStatus.Ongoing };
            var creator = new SpeedDatingParticipant { UserId = 1, SpeedDatingEventId = 100, IsCreator = true };

            _mockSpeedDateRepository.Setup(repo => repo.GetByIdAsync(100)).ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(100)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("already started"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_EventAlreadyEnded_ReturnsError()
        {
            // Arrange
            var command = new StartSpeedDatingEventCommand(1, 100);
            var speedDate = new SpeedDatingEvent { Id = 100, Status = DateStatus.Completed };
            var creator = new SpeedDatingParticipant { UserId = 1, SpeedDatingEventId = 100, IsCreator = true };

            _mockSpeedDateRepository.Setup(repo => repo.GetByIdAsync(100)).ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(100)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("already ended"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_EventCancelled_ReturnsError()
        {
            // Arrange
            var command = new StartSpeedDatingEventCommand(1, 100);
            var speedDate = new SpeedDatingEvent { Id = 100, Status = DateStatus.Cancelled };
            var creator = new SpeedDatingParticipant { UserId = 1, SpeedDatingEventId = 100, IsCreator = true };

            _mockSpeedDateRepository.Setup(repo => repo.GetByIdAsync(100)).ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(100)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains($"Speed date session {command.SpeedDateId} has already ended."))), Times.Once);
        }
    }
}
