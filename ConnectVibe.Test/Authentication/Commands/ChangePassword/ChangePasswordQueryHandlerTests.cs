using Fliq.Application.Authentication.Commands.ChangePassword;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Security;
using MapsterMapper;
using Moq;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;

namespace Fliq.Test.Authentication.Commands.ChangePassword
{
    [TestClass]
    public class ChangePasswordQueryHandlerTests
    {
        private ChangePasswordQueryHandler? _handler;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IEmailService>? _emailServiceMock;
        private Mock<ILoggerManager>? _loggerManagerMock;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerManagerMock = new Mock<ILoggerManager>();
            

            _handler = new ChangePasswordQueryHandler(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _loggerManagerMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var command = new ChangePasswordCommand("johndoe@example.com", "oldPassword", "newPassword");

            _userRepositoryMock?.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidCredentials, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_InvalidOldPassword_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var command = new ChangePasswordCommand("johndoe@example.com", "wrongOldPassword", "newPassword");
            var user = new User { Email = command.Email, PasswordSalt = "salt", PasswordHash = PasswordHash.Create("correctOldPassword", "salt") };

            _userRepositoryMock?.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidCredentials, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidOldPassword_ChangesPasswordAndSendsEmail()
        {
            // Arrange
            var command = new ChangePasswordCommand("johndoe@example.com", "oldPassword", "newPassword");
            var user = new User { Email = command.Email, PasswordSalt = "salt", PasswordHash = PasswordHash.Create("oldPassword", "salt") };

            _userRepositoryMock?.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(user);

            _emailServiceMock?.Setup(service => service.SendEmailAsync(command.Email, "Password Changed", "Successfully changed Password!"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsTrue(result.Value);

            _userRepositoryMock?.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);
            _emailServiceMock?.Verify(service => service.SendEmailAsync(command.Email, "Password Changed", "Successfully changed Password!"), Times.Once);
        }
    }
}
