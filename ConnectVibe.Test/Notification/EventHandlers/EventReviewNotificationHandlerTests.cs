

using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Moq;
using Fliq.Application.Notifications.Common;
using Fliq.Application.Common.Interfaces.Persistence;

namespace Fliq.Test.Notification.EventHandlers
{
    [TestClass]
    public class EventReviewNotificationHandlerTests
    {
        private Mock<INotificationRepository>? _notificationRepositoryMock;
        private Mock<IPushNotificationService>? _firebaseServiceMock;
        private Mock<ILoggerManager>? _loggerMock;
        private NotificationEventHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _firebaseServiceMock = new Mock<IPushNotificationService>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new NotificationEventHandler(
                _notificationRepositoryMock.Object,
                _firebaseServiceMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidEventReview_SendsNotificationToOrganizer()
        {
            // Arrange
            var eventReviewEvent = new EventReviewSubmittedEvent(
                userId: 1,
                organizerId: 2,
                eventId: 100,
                rating: 5,
                comments: "Great event!",
                title: "New Event Review Submitted",
                message: "John Doe rated your event 'Tech Conference' with 5 stars. Comment: Great event!"
            );

            var organizerTokens = new List<string> { "organizer-token" };

            _notificationRepositoryMock?.Setup(repo => repo.GetDeviceTokensByUserIdAsync(eventReviewEvent.OrganizerId))
                .ReturnsAsync(organizerTokens);

            // Act
            await _handler.Handle(eventReviewEvent, CancellationToken.None);

            // Assert
            _firebaseServiceMock?.Verify(service => service.SendNotificationAsync(
                "New Event Review Submitted",
                "John Doe rated your event 'Tech Conference' with 5 stars. Comment: Great event!",
                organizerTokens,
                eventReviewEvent.OrganizerId,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Once);

            _loggerMock?.Verify(logger => logger.LogInfo(
                $"Notification sent to Event Organizer: {eventReviewEvent.OrganizerId} for EventId: {eventReviewEvent.EventId}"
            ), Times.Once);
        }

        [TestMethod]
        public async Task Handle_NoDeviceTokens_DoesNotSendNotification()
        {
            // Arrange
            var eventReviewEvent = new EventReviewSubmittedEvent(
                userId: 1,
                organizerId: 2,
                eventId: 100,
                rating: 4,
                comments: "Good event!",
                title: "New Event Review Submitted",
                message: "John Doe rated your event 'Tech Meetup' with 4 stars. Comment: Good event!"
            );

            _notificationRepositoryMock?.Setup(repo => repo.GetDeviceTokensByUserIdAsync(eventReviewEvent.OrganizerId))
                .ReturnsAsync(new List<string>());

            // Act
            await _handler.Handle(eventReviewEvent, CancellationToken.None);

            // Assert
            _firebaseServiceMock?.Verify(service => service.SendNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Never);

            _loggerMock?.Verify(logger => logger.LogInfo(
                $"No registered device tokens for UserId: {eventReviewEvent.OrganizerId}"
            ), Times.Once);
        }

    }
}
