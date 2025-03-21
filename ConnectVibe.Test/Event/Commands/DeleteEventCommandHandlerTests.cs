using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using MediatR;
using Moq;

namespace Fliq.Application.Event.Commands.DeleteEvent.Tests
{
    [TestClass]
    public class DeleteEventCommandHandlerTests
    {
        
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IUserRepository>? _userRepositoryMock;        
        private Mock<IEventRepository>? _eventRepositoryMock;   
        private Mock<IMediator>? _mediatorMock;  
        private DeleteEventCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
           
            _loggerMock = new Mock<ILoggerManager>();
            _userRepositoryMock = new Mock<IUserRepository>();           
            _eventRepositoryMock = new Mock<IEventRepository>();         
            _mediatorMock = new Mock<IMediator>();
            

            _handler = new DeleteEventCommandHandler(

                _loggerMock.Object,
                _userRepositoryMock.Object,
                _eventRepositoryMock.Object,
                _mediatorMock.Object);
                
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new DeleteEventCommand(1);
            _eventRepositoryMock?.Setup(x => x.GetEventById(1)).Returns((Events)null!);

            // Act
            var result = await _handler?.Handle(command, CancellationToken.None)!;

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
            _loggerMock?.Verify(x => x.LogError($"Event with ID: {command.EventId} was not found."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EventAlreadyDeleted_ReturnsEventDeletedAlreadyError()
        {
            // Arrange
            var command = new DeleteEventCommand(1);
            var eventEntity = new Events { Id = 1, IsDeleted = true };
            _eventRepositoryMock?.Setup(x => x.GetEventById(1)).Returns(eventEntity);

            // Act
            var result = await _handler?.Handle(command, CancellationToken.None)!;

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventDeletedAlready, result.FirstError);
            _loggerMock?.Verify(x => x.LogError($"Event with ID: {command.EventId} has been deleted already."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new DeleteEventCommand(1);
            var eventEntity = new Events { Id = 1, IsDeleted = false, UserId = 100 };
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
        public async Task Handle_ValidRequest_DeletesEventAndPublishesNotification()
        {
            // Arrange
            var command = new DeleteEventCommand(1);
            var eventEntity = new Events
            {
                Id = 1,
                IsDeleted = false,
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
            _loggerMock?.Verify(x => x.LogInfo($"Deleting event with ID: {command.EventId}"), Times.Once());
            _loggerMock?.Verify(x => x.LogInfo($"Event with ID: {command.EventId} was deleted"), Times.Once());

               _mediatorMock?.Verify(x => x.Publish(
                It.Is<EventCreatedEvent>(e =>
                    e.Title == "Event Deleted" &&
                    e.Message == $"Your event 'Test Event' has been deleted!" &&
                    e.UserId == 100),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}