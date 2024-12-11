using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common;
using Moq;
using Fliq.Application.Notifications.Common.EventCreatedEvents;

namespace Fliq.Test.Notification.EventHandlers
{
    [TestClass]
    public class TicketsPurchasedHandlerTests
    {

        private Mock<INotificationRepository> _notificationRepositoryMock;
        private Mock<INotificationService> _firebaseServiceMock;
        private Mock<ILoggerManager> _loggerMock;
        private NotificationEventHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _firebaseServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new NotificationEventHandler(
                _notificationRepositoryMock.Object,
                _firebaseServiceMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_TicketPurchase_NotifiesOrganizerAndBuyer()
        {
            // Arrange

            string EventDate = "December 10, 2024";

            var eventCreatedEvent = new TicketPurchasedEvent(
                buyerId: 1,
                organizerId: 2,
                eventId: 100,
                numberOfTickets: 3,
                eventTitle: "Test Event",
                title: "TicketPurchased",
                message: $"You have successfully purchased {3} ticket(s) for the event 'Test Event' on {EventDate} .",
                eventDate: EventDate,
                buyerName: "JohnDoe"
            );

            var organizerToken = "organizer-token";
            var buyerToken = "buyer-token-1";

            var organizerTokens = new List<string> { organizerToken };
            var buyerTokens = new List<string> { buyerToken };

            // Mock device tokens for organizer and invitees
            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(2))
                .ReturnsAsync(new List<string> { organizerToken }); // Organizer tokens
            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(1))
                .ReturnsAsync(new List<string> { buyerToken }); // First invitee tokens


            // Act
            await _handler.Handle(eventCreatedEvent, CancellationToken.None);

            // Assert

            // Verify notification sent to the organizer
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
               "New Ticket Purchased",
                $"JohnDoe purchased {3} ticket(s) for your event 'Test Event'.",
                organizerTokens,
                2,
                null,
                null,
                null), Times.Once);

            // Verify notification sent to first invitee
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                "Ticket Purchase Successful!",
                $"You have successfully purchased {3} " +
                $"ticket(s) for the event 'Test Event' on {EventDate}.",
                buyerTokens,
                1,
                null,
                null,
                null), Times.Once);

        }
    }
}
