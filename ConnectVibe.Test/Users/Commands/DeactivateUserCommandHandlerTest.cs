

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Users.Commands;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Moq;

namespace Fliq.Test.Users.Commands
{
   [TestClass]
    public class DeactivateUserCommandHandlerTest
    {
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private Mock<IAuditTrailService>? _auditTrailService;
        private DeactivateUserCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();
            _auditTrailService = new Mock<IAuditTrailService>();

            _handler = new DeactivateUserCommandHandler(
                _mockUserRepository.Object,
                _mockLoggerManager.Object,
                _auditTrailService.Object
            );
        }

        [TestMethod]
        public async Task Handle_UserExists_DeactivatesUserSuccessfully()
        {
            // Arrange
            var userId = 1;
            var AdminUserId = 1;
            var user = new User { Id = userId, IsActive = true };

            _mockUserRepository
                .Setup(repo => repo.GetUserById(userId))
                .Returns(user); // User exists

            // Act
            var result = await _handler.Handle(new DeactivateUserCommand(userId, AdminUserId), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.IsFalse(user.IsActive); // User should be deactivated

            _mockUserRepository.Verify(repo => repo.Update(user), Times.Once);
            _mockLoggerManager.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Deactivated successfully"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var userId = 99;
            var AdminUserId = 100;

            _mockUserRepository
                .Setup(repo => repo.GetUserById(userId))
                .Returns((User)null); // User does not exist

            // Act
            var result = await _handler.Handle(new DeactivateUserCommand(userId, AdminUserId), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);

            _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _mockLoggerManager.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains($"User with ID {userId} not found"))), Times.Once);
        }
    }
}
