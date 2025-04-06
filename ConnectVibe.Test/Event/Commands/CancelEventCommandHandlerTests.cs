using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.CancelEvent;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;
using Moq;

namespace Fliq.Application.Tests.Event.Commands
{
    [TestClass]
    public class CancelEventCommandHandlerTests
    {
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IEventRepository>? _eventRepositoryMock;
        private CancelEventCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILoggerManager>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _handler = new CancelEventCommandHandler(
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _eventRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_ValidEvent_CancelsEventSuccessfully()
        {
            // Arrange
            var command = new CancelEventCommand(EventId: 1);
            var eventFromDb = new Events
            {
                Id = 1,
                UserId = 100,
                Status = EventStatus.Upcoming // Not yet cancelled
            };
            var user = new User { Id = 100 }; // Minimal User class

            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventFromDb);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(100)).Returns(user);
            _eventRepositoryMock.Setup(repo => repo.Update(It.IsAny<Events>()));
            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));
            _loggerMock.Setup(logger => logger.LogError(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(Unit.Value, result.Value);
            Assert.AreEqual(EventStatus.Cancelled, eventFromDb.Status); // Status should be updated to Cancelled

            _loggerMock.Verify(logger => logger.LogInfo($"Cancelling Event with ID: {command.EventId}"), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo($"Event with ID: {command.EventId} was cancelled"), Times.Once());
            _eventRepositoryMock.Verify(repo => repo.Update(eventFromDb), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new CancelEventCommand(EventId: 1);

            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns((Events)null!);
            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));
            _loggerMock.Setup(logger => logger.LogError(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);

            _loggerMock.Verify(logger => logger.LogInfo($"Cancelling Event with ID: {command.EventId}"), Times.Once());
            _loggerMock.Verify(logger => logger.LogError($"Event with ID: {command.EventId} was not found."), Times.Once());
            _eventRepositoryMock.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Never());
        }

        [TestMethod]
        public async Task Handle_EventAlreadyCancelled_ReturnsEventCancelledAlreadyError()
        {
            // Arrange
            var command = new CancelEventCommand(EventId: 1);
            var eventFromDb = new Events
            {
                Id = 1,
                UserId = 100,
                Status = EventStatus.Cancelled // Already cancelled
            };

            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventFromDb);
            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));
            _loggerMock.Setup(logger => logger.LogError(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventCancelledAlready, result.FirstError);

            _loggerMock.Verify(logger => logger.LogInfo($"Cancelling Event with ID: {command.EventId}"), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo($"Event with ID: {command.EventId} has been cancelled already."), Times.Once());
            _eventRepositoryMock.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Never());
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new CancelEventCommand(EventId: 1);
            var eventFromDb = new Events
            {
                Id = 1,
                UserId = 100,
                Status = EventStatus.Upcoming // Not yet cancelled
            };

            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventFromDb);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(100)).Returns((User)null!);
            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));
            _loggerMock.Setup(logger => logger.LogError(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);

            _loggerMock.Verify(logger => logger.LogInfo($"Cancelling Event with ID: {command.EventId}"), Times.Once());
            _loggerMock.Verify(logger => logger.LogError($"User with Id: {eventFromDb.UserId} was not found."), Times.Once());
            _eventRepositoryMock.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Never());
        }
    }
   
}