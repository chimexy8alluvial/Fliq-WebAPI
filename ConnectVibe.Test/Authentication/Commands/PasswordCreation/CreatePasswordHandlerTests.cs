using Fliq.Application.Authentication.Commands.PasswordCreation;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Moq;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;

namespace Fliq.Test.Authentication.Commands.PasswordCreation
{
    [TestClass]
    public class CreatePasswordHandlerTests
    {
        private CreatePasswordHandler _handler;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IOtpService> _otpServiceMock;
        private Mock<ILoggerManager> _loggerManagerMock;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _otpServiceMock = new Mock<IOtpService>();
            _loggerManagerMock = new Mock<ILoggerManager>();

            _handler = new CreatePasswordHandler(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _otpServiceMock.Object,
                _loggerManagerMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var command = new CreatePasswordCommand(1, "newPassword", "confirmPassword", "otp");

            _userRepositoryMock.Setup(repo => repo.GetUserById(command.Id))
                .Returns((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidCredentials, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_InvalidOtp_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var command = new CreatePasswordCommand(1, "newPassword", "confirmPassword", "invalidOtp");
            var user = new User { Id = command.Id, Email = "test@example.com" };

            _userRepositoryMock.Setup(repo => repo.GetUserById(command.Id))
                .Returns(user);

            _otpServiceMock.Setup(service => service.OtpExistAsync(user.Email, command.Otp))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidCredentials, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidOtp_CreatesPasswordAndSendsEmail()
        {
            // Arrange
            var command = new CreatePasswordCommand(1, "newPassword", "confirmPassword", "validOtp");
            var user = new User { Id = command.Id, Email = "test@example.com" };

            _userRepositoryMock.Setup(repo => repo.GetUserById(command.Id))
                .Returns(user);

            _otpServiceMock.Setup(service => service.OtpExistAsync(user.Email, command.Otp))
                .ReturnsAsync(true);

            _emailServiceMock.Setup(service => service.SendEmailAsync(user.Email, "Password Creation", "Successfully created Password!"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsTrue(result.Value);

            _userRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);
            _emailServiceMock.Verify(service => service.SendEmailAsync(user.Email, "Password Creation", "Successfully created Password!"), Times.Once);
        }
    }
}
