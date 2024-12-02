using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Notifications.Common;
using Fliq.Application.Notifications.Common.MatchEvents;
using Moq;

namespace Fliq.Test.Notification.MatchHandlers
{
    [TestClass]
    public class MatchRequestEventHandlerTest
    {
        private NotificationEventHandler _handler;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<INotificationRepository> _notificationRepositoryMock;
        private Mock<INotificationService> _firebaseServiceMock;
        private Mock<ILoggerManager> _loggerManagerMock;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _firebaseServiceMock = new Mock<INotificationService>();
            _loggerManagerMock = new Mock<ILoggerManager>();

            _handler = new NotificationEventHandler(
                _notificationRepositoryMock.Object,
                _firebaseServiceMock.Object,
                _loggerManagerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ValidEvent_SendsNotificationsToBothUsers()
        {
            // Arrange
            var notification = new MatchRequestEvent(1, 2, "url-to-image2", "url-to-image1", "John");

            var token1 = "token1";
            var token2 = "token2";

            // Mock the repository to return tokens for both users
            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(1))
                .ReturnsAsync(new List<string> { token1 }); // Initiator tokens
            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(2))
                .ReturnsAsync(new List<string> { token2 }); // Accepter tokens

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert

            // Verify notification sent to initiator
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                "Match Request Sent",
                "You have initiated a match request.",
                It.Is<List<string>>(tokens => tokens.Contains(token1)),
                1,
                null, // Confirm that the correct image URL is passed
               null,
                null), Times.Once);

            // Verify notification sent to accepter
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                "New Match Request",
                "John has sent you a match request.",
                It.Is<List<string>>(tokens => tokens.Contains(token2)),
                2,
                "url-to-image1", // Confirm that the correct image URL is passed
                null,
                null), Times.Once);
        }

        [TestMethod]
        public async Task Handle_NoDeviceTokens_DoesNotSendNotifications()
        {
            // Arrange
            var notification = new MatchRequestEvent(1, 2);

            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<string>());

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
