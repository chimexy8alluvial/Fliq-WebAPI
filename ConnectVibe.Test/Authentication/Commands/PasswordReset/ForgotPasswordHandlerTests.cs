using ConnectVibe.Application.Authentication.Commands.PasswordReset;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using Moq;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Test.Authentication.Commands.PasswordReset
{
    [TestClass]
    public class ForgotPasswordHandlerTests
    {
        private ForgotPasswordHandler _handler;
        private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IOtpService> _otpServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ILoggerManager> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _otpServiceMock = new Mock<IOtpService>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILoggerManager>();

            _handler = new ForgotPasswordHandler(
                _jwtTokenGeneratorMock.Object,
                _userRepositoryMock.Object,
                _otpServiceMock.Object,
                _emailServiceMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var command = new ForgotPasswordCommand("johndoe@example.com");

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidCredentials, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidUser_GeneratesOtpAndSendsEmail()
        {
            // Arrange
            var command = new ForgotPasswordCommand("johndoe@example.com");
            var user = new User { Email = command.Email, Id = 1 };
            var expectedOtp = "123456";

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(user);

            _otpServiceMock.Setup(service => service.GetOtpAsync(user.Email, user.Id))
                .ReturnsAsync(expectedOtp);

            _emailServiceMock.Setup(service => service.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP is {expectedOtp}"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedOtp, result.Value.otp);
            Assert.AreEqual(user.Email, result.Value.email);

            _loggerMock.Verify(logger => logger.LogInfo($"{user.Email} recieved the following otp--{expectedOtp}"), Times.Once);
            _emailServiceMock.Verify(service => service.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP is {expectedOtp}"), Times.Once);
        }
    }
}
