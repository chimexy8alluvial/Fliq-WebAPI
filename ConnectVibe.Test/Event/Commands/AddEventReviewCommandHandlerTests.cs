using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.AddEventReview;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;
using Moq;

namespace Fliq.Test.Event.Commands
{
    [TestClass]
    public class AddEventReviewCommandHandlerTests
    {
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<IEventReviewRepository>? _eventReviewRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IMapper>? _mapperMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IMediator>? _mediatorMock;

        private AddEventReviewCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _eventReviewRepositoryMock = new Mock<IEventReviewRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AddEventReviewCommandHandler(
                _eventReviewRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _eventRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mediatorMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new AddEventReviewCommand { EventId = 1, UserId = 100, Rating = 4, Comments = "Great event!" };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
            _loggerMock?.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_AddsReviewSuccessfully()
        {
            // Arrange
            var command = new AddEventReviewCommand { EventId = 1, UserId = 100, Rating = 5, Comments = "Amazing experience!" };

            var eventDetails = new Events { Id = command.EventId };
            _eventRepositoryMock?.Setup(repo => repo.GetEventById(command.EventId)).Returns(eventDetails);

            var newReview = new EventReview();
            _eventReviewRepositoryMock?.Setup(repo => repo.Add(It.IsAny<EventReview>()))
                                      .Callback<EventReview>(review => newReview = review);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(command.EventId, newReview.EventId);
            Assert.AreEqual(command.UserId, newReview.UserId);
            Assert.AreEqual(command.Rating, newReview.Rating);
            Assert.AreEqual(command.Comments, newReview.Comments);

            _eventReviewRepositoryMock?.Verify(repo => repo.Add(It.IsAny<EventReview>()), Times.Once);
            _loggerMock?.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
        }
    }
}