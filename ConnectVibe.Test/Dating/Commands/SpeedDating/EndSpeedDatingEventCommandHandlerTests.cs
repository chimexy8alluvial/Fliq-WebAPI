using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands.SpeedDating;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using Moq;

namespace Fliq.Test.Dating.Commands.SpeedDating
{
    [TestClass]
    public class EndSpeedDatingEventCommandHandlerTests
    {
        private Mock<ISpeedDatingEventRepository>? _mockSpeedDateRepository;
        private Mock<ISpeedDateParticipantRepository>? _mockSpeedDateParticipantRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private EndSpeedDateCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSpeedDateRepository = new Mock<ISpeedDatingEventRepository>();
            _mockSpeedDateParticipantRepository = new Mock<ISpeedDateParticipantRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new EndSpeedDateCommandHandler(
                _mockSpeedDateRepository.Object,
                _mockSpeedDateParticipantRepository.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenSpeedDateNotFound()
        {
            // Arrange
            var command = new EndSpeedDatingEventCommand(1, 1);
            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((SpeedDatingEvent)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.BlindDateNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenSpeedDateNotOngoing()
        {
            // Arrange
            var command = new EndSpeedDatingEventCommand(1, 1);
            var speedDate = new SpeedDatingEvent { Id = 1, Status = DateStatus.Completed };
            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(speedDate);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.BlindDateNotOngoing, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenUserIsNotCreator()
        {
            // Arrange
            var command = new EndSpeedDatingEventCommand(1, 1);
            var speedDate = new SpeedDatingEvent { Id = 1, Status = DateStatus.Ongoing };
            var creator = new SpeedDatingParticipant { UserId = 2 }; // Different user ID

            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository?.Setup(repo => repo.GetCreatorByBlindDateId(1)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.NotSessionCreator, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ShouldEndSession_WhenValidRequest()
        {
            // Arrange
            var command = new EndSpeedDatingEventCommand(1, 1);
            var speedDate = new SpeedDatingEvent { Id = 1, Status = DateStatus.Ongoing };
            var creator = new SpeedDatingParticipant { UserId = 1 }; // Same as command user ID

            _mockSpeedDateRepository?.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(speedDate);
            _mockSpeedDateParticipantRepository?.Setup(repo => repo.GetCreatorByBlindDateId(1)).ReturnsAsync(creator);
            _mockSpeedDateRepository?.Setup(repo => repo.UpdateAsync(speedDate)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(speedDate.EndSessionTime.HasValue);
            Assert.AreEqual(DateStatus.Completed, speedDate.Status);
        }
    }
}
