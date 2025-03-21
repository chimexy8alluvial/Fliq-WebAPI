using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Application.Notifications.Common;
using Moq;

namespace Fliq.Test.Notification.MatchHandlers
{
    [TestClass]
    public class MatchAcceptedEventHandlers
    {
        private NotificationEventHandler? _handler;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<INotificationRepository>? _notificationRepositoryMock;
        private Mock<IPushNotificationService>? _firebaseServiceMock;
        private Mock<ILoggerManager>? _loggerManagerMock;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _firebaseServiceMock = new Mock<IPushNotificationService>();
            _loggerManagerMock = new Mock<ILoggerManager>();

            _handler = new NotificationEventHandler(
                _notificationRepositoryMock.Object,
                _firebaseServiceMock.Object,
                _loggerManagerMock.Object);
        }

        [TestMethod]
        public async Task Handle_MatchAcceptedEvent_SendsNotificationsToBothUsers()
        {
            // Arrange
            var matchAcceptedEvent = new MatchAcceptedEvent(
                userId: 2,
                matchInitiatorUserId: 1,
                accepterUserId: 2,
                accepterImageUrl: "url-to-accepter-image",
                initiatorImageUrl: "url-to-initiator-image");

            var token1 = "initiator-token";
            var token2 = "accepter-token";

            // Mock the repository to return tokens for both users
            _notificationRepositoryMock?.Setup(repo => repo.GetDeviceTokensByUserIdAsync(1))
                .ReturnsAsync(new List<string> { token1 }); // Tokens for initiator
            _notificationRepositoryMock?.Setup(repo => repo.GetDeviceTokensByUserIdAsync(2))
                .ReturnsAsync(new List<string> { token2 }); // Tokens for accepter

            // Act
            await _handler.Handle(matchAcceptedEvent, CancellationToken.None);

            // Assert

            // Verify notification sent to the match initiator
            _firebaseServiceMock?.Verify(service => service.SendNotificationAsync(
                "Match Accepted!",
                "Your match request has been accepted.",
                It.Is<List<string>>(tokens => tokens.Contains(token1)),
                1,
                "url-to-initiator-image",
                "",
                "View Profile"), Times.Once);

            // Verify notification sent to the accepter
            _firebaseServiceMock?.Verify(service => service.SendNotificationAsync(
                "Match Accepted!",
                "You have a new connection",
                It.Is<List<string>>(tokens => tokens.Contains(token2)),
                2,
                "url-to-accepter-image",
                "",
                "View Profile"), Times.Once);

            // Optionally, verify the log was written
            _loggerManagerMock?.Verify(logger => logger.LogInfo(
                "Notification sent for match acceptance to AccepterUserId: 2 and InitiatorUserId: 1"), Times.Once);
        }
    }
}
