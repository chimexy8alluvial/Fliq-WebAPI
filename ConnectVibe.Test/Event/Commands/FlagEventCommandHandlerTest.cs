using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using MediatR;
using Moq;

namespace Fliq.Application.Event.Commands.FlagEvent.Tests
{
    [TestClass]
    public class FlagEventCommandHandlerTests
    {        
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IEventRepository>? _eventRepositoryMock;
        private FlagEventCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILoggerManager>();
            _userRepositoryMock = new Mock<IUserRepository>();           
            _eventRepositoryMock = new Mock<IEventRepository>();           

           

            _handler = new FlagEventCommandHandler( 
                _loggerMock.Object,
                _userRepositoryMock.Object,             
                _eventRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new FlagEventCommand(1);
            _eventRepositoryMock?.Setup(x => x.GetEventById(1)).Returns((Events)null!);

            // Act
            var result = await _handler?.Handle(command, CancellationToken.None)!;

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
            _loggerMock?.Verify(x => x.LogError($"Event with ID: {command.EventId} was not found."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EventAlreadyFlagged_ReturnsEventFlaggedAlreadyError()
        {
            // Arrange
            var command = new FlagEventCommand(1);
            var eventEntity = new Events { Id = 1, IsFlagged = true };
            _eventRepositoryMock?.Setup(x => x.GetEventById(1)).Returns(eventEntity);

            // Act
            var result = await _handler?.Handle(command, CancellationToken.None)!;

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventFlaggedAlready, result.FirstError);
            _loggerMock?.Verify(x => x.LogInfo($"Event with ID: {command.EventId} has been flagged already."), Times.Once());
        }

       
        [TestMethod]
        public async Task Handle_ValidRequest_FlagsEventAndPublishesNotification()
        {
            // Arrange
            var command = new FlagEventCommand(1);
            var eventEntity = new Events
            {
                Id = 1,
                IsFlagged = false,
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
            _loggerMock?.Verify(x => x.LogInfo($"Flagging Event with ID: {command.EventId}"), Times.Once());
            _loggerMock?.Verify(x => x.LogInfo($"Event with ID: {command.EventId} was flagged"), Times.Once());

           
        }
    }
}