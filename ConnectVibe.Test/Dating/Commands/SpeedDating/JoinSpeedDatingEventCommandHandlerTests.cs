using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands.SpeedDating;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Infrastructure.Persistence.Repositories;
using Moq;

namespace Fliq.Test.Dating.Commands.SpeedDating
{
    [TestClass]
    public class JoinSpeedDatingEventCommandHandlerTests
    {
        private Mock<ISpeedDatingEventRepository>? _mockSpeedDateRepository;
        private Mock<ISpeedDateParticipantRepository>? _mockSpeedDateParticipantRepository;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private JoinSpeedDatingEventCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSpeedDateRepository = new Mock<ISpeedDatingEventRepository>();
            _mockSpeedDateParticipantRepository = new Mock<ISpeedDateParticipantRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();

            _handler = new JoinSpeedDatingEventCommandHandler(
                _mockSpeedDateRepository.Object,
                _mockSpeedDateParticipantRepository.Object,
                _mockLoggerManager.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_UserJoinsSuccessfully()
        {
            // Arrange
            var command = new JoinSpeedDatingEventCommand(UserId: 1001, SpeedDateId: 1);
            var speedDate = new SpeedDatingEvent { Id = 1, MaxParticipants = 10 };

            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(command.SpeedDateId))
                .ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository?.Setup(repo => repo.GetByUserAndSpeedDateId(command.UserId, command.SpeedDateId))
                .ReturnsAsync((SpeedDatingParticipant?)null);
            _mockSpeedDateParticipantRepository?.Setup(repo => repo.CountByBlindDateId(command.SpeedDateId))
                .ReturnsAsync(5);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _mockSpeedDateParticipantRepository?.Verify(repo => repo.AddAsync(It.IsAny<SpeedDatingParticipant>()), Times.Once);
            _mockLoggerManager?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("successfully joined"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_SpeedDateNotFound_ReturnsError()
        {
            // Arrange
            var command = new JoinSpeedDatingEventCommand(UserId: 1001, SpeedDateId: 1);
            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(command.SpeedDateId))
                .ReturnsAsync((SpeedDatingEvent?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(result.FirstError, Errors.Dating.BlindDateNotFound);
            _mockLoggerManager?.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("not found"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_SessionAlreadyEnded_ReturnsError()
        {
            // Arrange
            var command = new JoinSpeedDatingEventCommand(UserId: 1001, SpeedDateId: 1);
            var speedDate = new SpeedDatingEvent { Id = 1, EndSessionTime = DateTime.UtcNow.AddHours(-1) };

            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(command.SpeedDateId))
                .ReturnsAsync(speedDate);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(result.FirstError, Errors.Dating.BlindDateSessionEnded);
            _mockLoggerManager?.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("already ended"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_UserAlreadyJoined_ReturnsError()
        {
            // Arrange
            var command = new JoinSpeedDatingEventCommand(UserId: 1001, SpeedDateId: 1);
            var speedDate = new SpeedDatingEvent { Id = 1 };
            var existingParticipant = new SpeedDatingParticipant { SpeedDatingEventId = 1, UserId = 1001 };

            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(command.SpeedDateId))
                .ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository?.Setup(repo => repo.GetByUserAndSpeedDateId(command.UserId, command.SpeedDateId))
                .ReturnsAsync(existingParticipant);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(result.FirstError, Errors.Dating.AlreadyJoined);
            _mockLoggerManager?.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("already a participant"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_SessionIsFull_ReturnsError()
        {
            // Arrange
            var command = new JoinSpeedDatingEventCommand(UserId: 1001, SpeedDateId: 1);
            var speedDate = new SpeedDatingEvent { Id = 1, MaxParticipants = 5 };

            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(command.SpeedDateId))
                .ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository?.Setup(repo => repo.GetByUserAndSpeedDateId(command.UserId, command.SpeedDateId))
                .ReturnsAsync((SpeedDatingParticipant?)null);
            _mockSpeedDateParticipantRepository?.Setup(repo => repo.CountByBlindDateId(command.SpeedDateId))
                .ReturnsAsync(5);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(result.FirstError, Errors.Dating.BlindDateFull);
            _mockLoggerManager?.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("reached its maximum participants"))), Times.Once);
        }
    }
}
