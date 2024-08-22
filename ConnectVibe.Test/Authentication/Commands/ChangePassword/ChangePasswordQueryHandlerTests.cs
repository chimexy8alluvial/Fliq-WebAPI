using ConnectVibe.Application.Authentication.Commands.ChangePassword;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Security;
using MapsterMapper;
using Moq;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Test.Authentication.Commands.ChangePassword
{
    [TestClass]
    public class ChangePasswordQueryHandlerTests
    {
        private ChangePasswordQueryHandler _handler;
        private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IOtpRepository> _otpRepositoryMock;
        private Mock<IOtpService> _otpServiceMock;

        [TestInitialize]
        public void Setup()
        {
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _otpRepositoryMock = new Mock<IOtpRepository>();
            _otpServiceMock = new Mock<IOtpService>();

            _handler = new ChangePasswordQueryHandler(
                _jwtTokenGeneratorMock.Object,
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _emailServiceMock.Object,
                _otpRepositoryMock.Object,
                _otpServiceMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var command = new ChangePasswordCommand("johndoe@example.com", "oldPassword", "newPassword");

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns((User)null);

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

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
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

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(user);

            _emailServiceMock.Setup(service => service.SendEmailAsync(command.Email, "Password Changed", "Successfully changed Password!"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsTrue(result.Value);

            _userRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);
            _emailServiceMock.Verify(service => service.SendEmailAsync(command.Email, "Password Changed", "Successfully changed Password!"), Times.Once);
        }
    }
}
