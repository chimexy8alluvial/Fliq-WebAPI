using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Application.Notifications.Common;
using Moq;

namespace Fliq.Test.Notification.EventHandlers
{
    public class EventCreatedHandlerTest
    {
        [TestClass]
        public class EventCreatedEventHandlerTest
        {
            private NotificationEventHandler _handler;
            private Mock<INotificationRepository> _notificationRepositoryMock;
            private Mock<INotificationService> _firebaseServiceMock;
            private Mock<ILoggerManager> _loggerManagerMock;

            [TestInitialize]
            public void Setup()
            {
                _notificationRepositoryMock = new Mock<INotificationRepository>();
                _firebaseServiceMock = new Mock<INotificationService>();
                _loggerManagerMock = new Mock<ILoggerManager>();

                _handler = new NotificationEventHandler(
                    _notificationRepositoryMock.Object,
                    _firebaseServiceMock.Object,
                    _loggerManagerMock.Object);
            }

            [TestMethod]
            public async Task Handle_ValidEvent_NotifiesOrganizerAndInvitees()
            {
                // Arrange
                var eventCreatedEvent = new EventCreatedEvent(
                    userId: 1,
                    eventId: 100,
                    organizerId: 1,
                    organizerName: "John Doe",
                    inviteeIds: new List<int> { 2, 3 },
                    title: "Birthday Party",
                    message: "Your event 'Birthday Party' has been successfully created!"
                );

                var organizerToken = "organizer-token";
                var inviteeToken1 = "invitee-token-1";
                var inviteeToken2 = "invitee-token-2";

                var organizerTokens = new List<string> { organizerToken };
                var Invitee1Tokens = new List<string> { inviteeToken1 };
                var Invitee2Tokens = new List<string> { inviteeToken2 };

                // Mock device tokens for organizer and invitees
                _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(1))
                    .ReturnsAsync(new List<string> { organizerToken }); // Organizer tokens
                _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(2))
                    .ReturnsAsync(new List<string> { inviteeToken1 }); // First invitee tokens
                _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(3))
                    .ReturnsAsync(new List<string> { inviteeToken2 }); // Second invitee tokens

                // Act
                await _handler.Handle(eventCreatedEvent, CancellationToken.None);

                // Assert

                // Verify notification sent to the organizer
                _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                    "Birthday Party",
                    "Your event 'Birthday Party' has been successfully created!",
                    organizerTokens,
                    1,
                    null,
                    null,
                    "View Event"), Times.Once);

                // Verify notification sent to first invitee
                _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                    "You're Invited!",
                    "John Doe has invited you to the event 'Birthday Party'.",
                    Invitee1Tokens,
                    2,
                    null,
                    null,
                    "View Invitation"), Times.Once);

                // Verify notification sent to second invitee
                _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                    "You're Invited!",
                    "John Doe has invited you to the event 'Birthday Party'.",
                    Invitee2Tokens,
                    3,
                    null,
                    null,
                    "View Invitation"), Times.Once);
            }

            [TestMethod]
            public async Task Handle_NoDeviceTokens_DoesNotSendNotifications()
            {
                // Arrange
                var eventCreatedEvent = new EventCreatedEvent(
                    userId: 1,
                    eventId: 100,
                    organizerId: 1,
                    organizerName: "John Doe",
                    inviteeIds: new List<int> { 2, 3 },
                    title: "Event Created",
                    message: "Your event 'Birthday Party' has been successfully created!"
                );

                // Mock no tokens for organizer or invitees
                _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(new List<string>()); // Empty list for all users

                // Act
                await _handler.Handle(eventCreatedEvent, CancellationToken.None);

                // Assert
                _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never);
            }
        }
    }
}
