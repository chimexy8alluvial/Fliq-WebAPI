using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Notifications.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Moq;

namespace Fliq.Test.Notification.EventHandlers
{
    [TestClass]
    public class TicketsPurchasedHandlerTests
    {
        private Mock<INotificationRepository>? _notificationRepositoryMock;
        private Mock<IPushNotificationService>? _firebaseServiceMock;
        private Mock<IEmailService>? _emailServiceMock;
        private Mock<ILoggerManager>? _loggerMock;
        private NotificationEventHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _firebaseServiceMock = new Mock<IPushNotificationService>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new NotificationEventHandler(
                _notificationRepositoryMock.Object,
                _firebaseServiceMock.Object,
                _emailServiceMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_TicketPurchasedEvent_NotifiesBuyerOrganizerAndAssignees()
        {
            // Arrange
            var ticketDetails = new List<TicketDetail>
            {
                new TicketDetail { UserId = 100, TicketCategory = "Regular" }, // Buyer (will be skipped in TicketDetails loop)
                new TicketDetail { UserId = 200, TicketCategory = "Vip" },     // Another user
                new TicketDetail { Email = "guest@example.com", TicketCategory = "VVip" } // Guest via email
            };

            var ticketPurchasedEvent = new TicketPurchasedEvent(
                buyerId: 100,
                organizerId: 300,
                eventId: 1,
                numberOfTickets: 3,
                eventTitle: "Test Event",
                eventDate: "December 10, 2024",
                buyerName: "John Doe",
                ticketDetails: ticketDetails,
                title: "TicketPurchased",
                message: "You have successfully purchased 3 ticket(s) for 'Test Event' on December 10, 2024."
            );

            var buyerTokens = new List<string> { "buyer-token-1" };
            var organizerTokens = new List<string> { "organizer-token" };
            var user200Tokens = new List<string> { "user-200-token" };

            // Mock device token retrieval
            _notificationRepositoryMock!.Setup(repo => repo.GetDeviceTokensByUserIdAsync(100))
                .ReturnsAsync(buyerTokens);
            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(300))
                .ReturnsAsync(organizerTokens);
            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(200))
                .ReturnsAsync(user200Tokens);

            // Mock push notification service
            _firebaseServiceMock!.Setup(service => service.SendNotificationAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Mock email service
            _emailServiceMock!.Setup(service => service.SendEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler!.Handle(ticketPurchasedEvent, CancellationToken.None);

            // Assert
            // 1. Buyer notification (push)
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                "Ticket Purchase Successful!",
                "You have successfully purchased 3 ticket(s) for the event 'Test Event' on December 10, 2024.",
                buyerTokens,
                100,
                null, // ImageUrl
                null, // ActionUrl (could be ticketPurchasedEvent.ActionUrl if defined)
                null  // ButtonText (could be ticketPurchasedEvent.ButtonText if defined)
            ), Times.Once());

            // 2. Organizer notification (push)
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                "New Ticket Purchased",
                "John Doe purchased 3 ticket(s) for your event 'Test Event'.",
                organizerTokens,
                300,
                null,
                null,
                null
            ), Times.Once());

            // 3. Individual notification for User 200 (push)
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                "Ticket Assigned",
                "You’ve been assigned a Vip ticket for 'Test Event' on December 10, 2024.",
                user200Tokens,
                200,
                null,
                null,
                null
            ), Times.Once());

            // 4. Email notification for guest@example.com
            _emailServiceMock.Verify(service => service.SendEmailAsync(
                "guest@example.com",
                "Ticket Assigned",
                "You’ve been assigned a VVip ticket for 'Test Event' on December 10, 2024."
            ), Times.Once());

            // 5. Logging
            _loggerMock!.Verify(logger => logger.LogInfo(
                "Notifications sent for ticket purchase: BuyerId 100, OrganizerId 300."), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo(
                "Email sent to guest@example.com for ticket assignment."), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo(
                It.Is<string>(msg => msg.StartsWith("Device tokens retrieved for UserId:") && msg.Contains("token"))),
                Times.Exactly(3)); // Buyer, Organizer, User 200
            _loggerMock.Verify(logger => logger.LogInfo(
                It.Is<string>(msg => msg.StartsWith("Notification sent for event to UserId:"))),
                Times.Exactly(3)); // Buyer, Organizer, User 200
        }

        [TestMethod]
        public async Task Handle_TicketPurchasedEvent_NoDeviceTokens_LogsAndSkips()
        {
            // Arrange
            var ticketPurchasedEvent = new TicketPurchasedEvent(
                buyerId: 100,
                organizerId: 300,
                eventId: 1,
                numberOfTickets: 1,
                eventTitle: "Test Event",
                eventDate: "December 10, 2024",
                buyerName: "John Doe",
                ticketDetails: null!,
                title: "TicketPurchased",
                message: "You have successfully purchased 1 ticket(s) for 'Test Event' on December 10, 2024."
            );

            _notificationRepositoryMock!.Setup(repo => repo.GetDeviceTokensByUserIdAsync(100))
                .ReturnsAsync(new List<string>()); // No tokens for buyer
            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(300))
                .ReturnsAsync(new List<string>()); // No tokens for organizer

            // Act
            await _handler!.Handle(ticketPurchasedEvent, CancellationToken.None);

            // Assert
            _firebaseServiceMock!.Verify(service => service.SendNotificationAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());

            _loggerMock!.Verify(logger => logger.LogInfo("No registered device tokens for UserId: 100"), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo("No registered device tokens for UserId: 300"), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo(
                "Notifications sent for ticket purchase: BuyerId 100, OrganizerId 300."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_TicketPurchasedEvent_NoTicketDetails_NotifiesOnlyBuyerAndOrganizer()
        {
            // Arrange
            var ticketPurchasedEvent = new TicketPurchasedEvent(
                buyerId: 100,
                organizerId: 300,
                eventId: 1,
                numberOfTickets: 1,
                eventTitle: "Test Event",
                eventDate: "December 10, 2024",
                buyerName: "John Doe",
                ticketDetails: null!,
                title: "TicketPurchased",
                message: "You have successfully purchased 1 ticket(s) for 'Test Event' on December 10, 2024."
            );

            var buyerTokens = new List<string> { "buyer-token-1" };
            var organizerTokens = new List<string> { "organizer-token" };

            _notificationRepositoryMock!.Setup(repo => repo.GetDeviceTokensByUserIdAsync(100))
                .ReturnsAsync(buyerTokens);
            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(300))
                .ReturnsAsync(organizerTokens);

            // Act
            await _handler!.Handle(ticketPurchasedEvent, CancellationToken.None);

            // Assert
            _firebaseServiceMock!.Verify(service => service.SendNotificationAsync(
                "Ticket Purchase Successful!",
                "You have successfully purchased 1 ticket(s) for the event 'Test Event' on December 10, 2024.",
                buyerTokens,
                100,
                null,
                null,
                null
            ), Times.Once());

            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                "New Ticket Purchased",
                "John Doe purchased 1 ticket(s) for your event 'Test Event'.",
                organizerTokens,
                300,
                null,
                null,
                null
            ), Times.Once());

            _emailServiceMock!.Verify(service => service.SendEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());

            _loggerMock!.Verify(logger => logger.LogInfo(
                "Notifications sent for ticket purchase: BuyerId 100, OrganizerId 300."), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo(
                It.Is<string>(msg => msg.Contains("Device tokens retrieved for UserId:") && msg.Contains("token"))),
                Times.Exactly(2)); // Buyer, Organizer
            _loggerMock.Verify(logger => logger.LogInfo(
                It.Is<string>(msg => msg.Contains("Notification sent for event to UserId:"))),
                Times.Exactly(2)); // Buyer, Organizer
        }
    }
}