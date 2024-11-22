
//using FirebaseAdmin.Messaging;
//using Fliq.Application.Common.Interfaces.Persistence;
//using Fliq.Application.Common.Interfaces.Services;
//using Fliq.Infrastructure.Services.NotificationServices.Firebase;
//using Moq;

//namespace Fliq.Test.Notification
//{
//    public class MockFireBaseNotificationService : FireBaseNotificationService
//    {
//        public MockFireBaseNotificationService(
//            ILoggerManager logger,
//            INotificationRepository notificationRepository,
//            IFirebaseMessagingWrapper firebaseWrapper)
//            : base(logger, notificationRepository, firebaseWrapper)
//        {
//            // No Firebase initialization
//        }
//    }

//    public class MockFirebaseMessagingWrapper : IFirebaseMessagingWrapper
//    {
//        public Task<BatchResponse> SendEachForMulticastAsync(MulticastMessage message)
//        {
//            // Simulate a Firebase error for testing purposes
//            throw new Exception("Firebase error");
//        }
//    }

//    [TestClass]
//    public class FireBaseNotificationServiceTest
//    {
//        private Mock<ILoggerManager> _loggerMock;
//        private Mock<INotificationRepository> _notificationRepositoryMock;
//        private MockFireBaseNotificationService _service;

//        [TestInitialize]
//        public void Setup()
//        {
//            _loggerMock = new Mock<ILoggerManager>();
//            _notificationRepositoryMock = new Mock<INotificationRepository>();

//            // Use the custom MockFirebaseMessagingWrapper
//            var mockFirebaseWrapper = new MockFirebaseMessagingWrapper();

//            _service = new MockFireBaseNotificationService(
//                _loggerMock.Object,
//                _notificationRepositoryMock.Object,
//                mockFirebaseWrapper);
//        }

//        [TestMethod]
//        public async Task SendNotificationAsync_FirebaseFailure_LogsError()
//        {
//            // Arrange
//            var title = "Test Title";
//            var message = "Test Message";
//            var deviceTokens = new List<string> { "token1", "token2" };
//            var userId = 1;

//            _notificationRepositoryMock.Setup(repo => repo.Add(It.IsAny<Domain.Entities.Notifications.Notification>()));

//            // Act
//            await _service.SendNotificationAsync(title, message, deviceTokens, userId);

//            // Assert
//            _loggerMock.Verify(logger => logger.LogError(
//                It.Is<string>(msg => msg.Contains("Error sending notification to UserId 1: Firebase error"))),
//                Times.Once);
//        }
//    }

//}
