using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Notifications.Commands.DeviceRegistration;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Notifications;
using Moq;

namespace Fliq.Test.Notification.Commands
{
    [TestClass]
    public class SaveDeviceTokensCommandHandlerTest
    {
        private SaveDeviceTokenCommandHandler _handler;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<INotificationRepository> _notificationRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _notificationRepositoryMock = new Mock<INotificationRepository>();

            _handler = new SaveDeviceTokenCommandHandler(
                 _notificationRepositoryMock.Object,
                 _userRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserDoesNotExist_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new SaveDeviceTokenCommand(123,"test-token");
            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_SavesDeviceTokenSuccessfully()
        {
            // Arrange
            var command = new SaveDeviceTokenCommand(123, "test-token");
            var user = new User { Id = 123 };
            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _notificationRepositoryMock.Verify(repo => repo.RegisterDeviceToken(It.Is<UserDeviceToken>(
                token => token.UserId == 123 && token.DeviceToken == "test-token")), Times.Once);
        }
    }
}
