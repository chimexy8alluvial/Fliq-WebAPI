using Fliq.Application.Authentication.Commands.ValidateOTP;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using MapsterMapper;
using Moq;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using StreamChat.Clients;

namespace Fliq.Test.Authentication.Commands.ValidateOTP
{
    [TestClass]
    public class ValidateOTPCommandHandlerTests
    {
        private ValidateOTPCommandHandler _handler;
        private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IOtpService> _otpServiceMock;
        private Mock<ILoggerManager> _loggerManagerMock;
        private Mock<StreamClientFactory> _streamClientFactoryMock;

        [TestInitialize]
        public void Setup()
        {
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _otpServiceMock = new Mock<IOtpService>();
            _loggerManagerMock = new Mock<ILoggerManager>();
            _streamClientFactoryMock = new Mock<StreamClientFactory>();
            

            _handler = new ValidateOTPCommandHandler(
                _jwtTokenGeneratorMock.Object,
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _emailServiceMock.Object,
                _otpServiceMock.Object,
                _loggerManagerMock.Object,
                _streamClientFactoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_InvalidOtp_ReturnsInvalidOtpError()
        {
            // Arrange
            var command = new ValidateOTPCommand("johndoe@example.com", "123456");

            _otpServiceMock.Setup(service => service.ValidateOtpAsync(command.Email, command.Otp))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidOTP, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidOtp_ValidatesUserEmailAndGeneratesToken()
        {
            // Arrange
            var command = new ValidateOTPCommand("johndoe@example.com", "123456");
            var user = new User { Email = command.Email, Id = 1, IsEmailValidated = false };
            var expectedToken = "valid-token";

            _otpServiceMock.Setup(service => service.ValidateOtpAsync(command.Email, command.Otp))
                .ReturnsAsync(true);

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(user);

            _jwtTokenGeneratorMock.Setup(generator => generator.GenerateToken(user))
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedToken, result.Value.Token);
            Assert.AreEqual(user.Email, result.Value.user.Email);
            Assert.IsTrue(user.IsEmailValidated);

            _userRepositoryMock.Verify(repo => repo.Update(user), Times.Once);
        }
    }
}
