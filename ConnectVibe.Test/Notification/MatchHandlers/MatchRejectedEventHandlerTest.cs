

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Application.Notifications.Common;
using Moq;

namespace Fliq.Test.Notification.MatchHandlers
{
    [TestClass]
    public class MatchRejectedEventHandlerTest
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
        public async Task Handle_MatchRejectedEvent_SendsNotificationToUser()
        {
            // Arrange
            var matchRejectedEvent = new MatchRejectedEvent(userId: 1);
            var token = "deviceToken1";

            _notificationRepositoryMock.Setup(repo => repo.GetDeviceTokensByUserIdAsync(1))
                .ReturnsAsync(new List<string> { token }); // User has tokens

            // Act
            await _handler.Handle(matchRejectedEvent, CancellationToken.None);

            // Assert
            _firebaseServiceMock.Verify(service => service.SendNotificationAsync(
                "Match Rejected",
                "You have rejected a match.",
                It.Is<List<string>>(tokens => tokens.Contains(token)),
                1,
                null, // No image for rejection event
                null, // No action URL
                null), Times.Once);

            _loggerManagerMock.Verify(logger => logger.LogInfo(
                It.Is<string>(msg => msg.Contains("Notification sent for match rejection to UserId: 1"))), Times.Once);
        }
    }
}
