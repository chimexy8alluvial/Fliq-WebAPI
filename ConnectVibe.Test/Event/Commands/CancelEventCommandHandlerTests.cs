using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.EventServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;
using Moq;

namespace Fliq.Application.Event.Commands.CancelEvent.Tests
{
    [TestClass]
    public class CancelEventCommandHandlerTests
    {
        private Mock<IMapper>? _mapperMock;
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IMediaServices>? _mediaServicesMock;
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ILocationService>? _locationServiceMock;
        private Mock<IMediator>? _mediatorMock;
        private Mock<IEmailService>? _emailServiceMock;
        private Mock<IEventService>?    _eventServiceMock;
        private CancelEventCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerManager>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mediaServicesMock = new Mock<IMediaServices>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _locationServiceMock = new Mock<ILocationService>();
            _mediatorMock = new Mock<IMediator>();
            _emailServiceMock = new Mock<IEmailService>();
            _eventServiceMock = new Mock<IEventService>();

            _handler = new CancelEventCommandHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _mediaServicesMock.Object,
                _eventRepositoryMock.Object,
                _locationServiceMock.Object,
                _mediatorMock.Object,
                _emailServiceMock.Object,
                _eventServiceMock.Object);
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new CancelEventCommand(1);
            _eventRepositoryMock?.Setup(x => x.GetEventById(1)).Returns((Events)null!);

            // Act
            var result = await _handler?.Handle(command, CancellationToken.None)!;

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
            _loggerMock?.Verify(x => x.LogError($"Event with ID: {command.EventId} was not found."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EventAlreadyCancelled_ReturnsEventCancelledAlreadyError()
        {
            // Arrange
            var command = new CancelEventCommand(1);
            var eventEntity = new Events { Id = 1, IsCancelled = true };
            _eventRepositoryMock?.Setup(x => x.GetEventById(1)).Returns(eventEntity);

            // Act
            var result = await _handler?.Handle(command, CancellationToken.None)!;

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventCancelledAlready, result.FirstError);
            _loggerMock?.Verify(x => x.LogError($"Event with ID: {command.EventId} has been cancelled already."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new CancelEventCommand(1);
            var eventEntity = new Events { Id = 1, IsCancelled = false, UserId = 100 };
            _eventRepositoryMock?.Setup(x => x.GetEventById(1)).Returns(eventEntity);
            _userRepositoryMock?.Setup(x => x.GetUserById(100)).Returns((User)null!);

            // Act
            var result = await _handler?.Handle(command, CancellationToken.None)!;

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
            _loggerMock?.Verify(x => x.LogError($"User with Id: {eventEntity.UserId} was not found."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidRequest_CancelsEventAndPublishesNotification()
        {
            // Arrange
            var command = new CancelEventCommand(1);
            var eventEntity = new Events
            {
                Id = 1,
                IsCancelled = false,
                UserId = 100,
                EventTitle = "Test Event"
            };
            var user = new User
            {
                Id = 100,
                FirstName = "John",
                LastName = "Doe"
            };

            _eventRepositoryMock?.Setup(x => x.GetEventById(1)).Returns(eventEntity);
            _userRepositoryMock?.Setup(x => x.GetUserById(100)).Returns(user);

            // Act
            var result = await _handler?.Handle(command, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(Unit.Value, result.Value);

            Assert.IsTrue(eventEntity.IsFlagged);
            _eventRepositoryMock?.Verify(x => x.Update(eventEntity), Times.Once());
            _loggerMock?.Verify(x => x.LogInfo($"Cancelling Event with ID: {command.EventId}"), Times.Once());
            _loggerMock?.Verify(x => x.LogInfo($"Event with ID: {command.EventId} was cancelled"), Times.Once());

            _mediatorMock?.Verify(x => x.Publish(
                It.Is<EventCreatedEvent>(e =>
                    e.Title == "Event Cancelled" &&
                    e.Message == $"Your event 'Test Event' has been Cancelled!" &&
                    e.UserId == 100),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}