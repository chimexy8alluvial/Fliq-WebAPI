using Fliq.Application.Authentication.Commands.ValidatePasswordOTP;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using MapsterMapper;
using Moq;


namespace Fliq.Test.Authentication.Commands.ValidatePasswordOTP
{
    [TestClass]
    public class ValidatePasswordOTPCommandHandlerTests
    {
        private ValidatePasswordOTPCommandHandler _handler;
        private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IOtpService> _otpServiceMock;

        [TestInitialize]
        public void Setup()
        {
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _otpServiceMock = new Mock<IOtpService>();

            _handler = new ValidatePasswordOTPCommandHandler(
                _jwtTokenGeneratorMock.Object,
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _emailServiceMock.Object,
                _otpServiceMock.Object);
        }

        [TestMethod]
        public async Task Handle_InvalidOtp_ReturnsInvalidOtpError()
        {
            // Arrange
            var command = new ValidatePasswordOTPCommand("johndoe@example.com", "123456");

            _otpServiceMock.Setup(service => service.ValidateOtpAsync(command.Email, command.Otp))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidOTP, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidOtp_ReturnsUserAndOtp()
        {
            // Arrange
            var command = new ValidatePasswordOTPCommand("johndoe@example.com", "123456");
            var user = new User { Email = command.Email, Id = 1 };

            _otpServiceMock.Setup(service => service.ValidateOtpAsync(command.Email, command.Otp))
                .ReturnsAsync(true);

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(user, result.Value.user);
            Assert.AreEqual(command.Otp, result.Value.otp);
        }
    }
}
