using Fliq.Application.Authentication.Commands.ValidateOTP;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Moq;
using Fliq.Domain.Common.Errors;
using StreamChat.Clients;
using StreamChat.Models;

namespace Fliq.Test.Authentication.Commands.ValidateOTP
{
    [TestClass]
    public class ValidateOTPCommandHandlerTests
    {
        private ValidateOTPCommandHandler? _handler;
        private Mock<IJwtTokenGenerator>? _jwtTokenGeneratorMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IOtpService>? _otpServiceMock;
        private Mock<IStreamClientFactory>? _streamClientFactoryMock;
        private Mock<ILoggerManager>? _loggerManagerMock;

        [TestInitialize]
        public void Setup()
        {
            // Set the STREAM_KEY environment variable for the test context
            System.Environment.SetEnvironmentVariable("STREAM_KEY", "dummy-key");

            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _otpServiceMock = new Mock<IOtpService>();
            _streamClientFactoryMock = new Mock<IStreamClientFactory>();
            _loggerManagerMock = new Mock<ILoggerManager>();

            // Mock Stream API behavior
            var mockUserClient = new Mock<IUserClient>();
            _streamClientFactoryMock
                .Setup(factory => factory.GetUserClient())
                .Returns(mockUserClient.Object);

            mockUserClient
                .Setup(client => client.CreateToken(It.IsAny<string>()))
                .Returns("mock-stream-token");

            mockUserClient
       .Setup(client => client.UpsertManyAsync(It.IsAny<List<UserRequest>>()))
       .ReturnsAsync(new UpsertResponse());

            _handler = new ValidateOTPCommandHandler(
                _jwtTokenGeneratorMock.Object,
                _userRepositoryMock.Object,
                _otpServiceMock.Object,
                _streamClientFactoryMock.Object,
                _loggerManagerMock.Object
                );

            
        }

        [TestMethod]
        public async Task Handle_InvalidOtp_ReturnsInvalidOtpError()
        {
            // Arrange
            var command = new ValidateOTPCommand("johndoe@example.com", "123456");

            _otpServiceMock?.Setup(service => service.ValidateOtpAsync(command.Email, command.Otp))
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
            var user = new Domain.Entities.User { Email = command.Email, Id = 1, IsEmailValidated = false, Role = new Domain.Entities.Role { Name = "User", Id = 1} };
            var expectedToken = "valid-token";

            _otpServiceMock?.Setup(service => service.ValidateOtpAsync(command.Email, command.Otp))
                .ReturnsAsync(true);

            _userRepositoryMock?.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(user);

            _jwtTokenGeneratorMock?.Setup(generator => generator.GenerateToken(user))
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedToken, result.Value.Token);
            Assert.AreEqual("mock-stream-token", result.Value.StreamToken); // Validate Stream token
            Assert.AreEqual(user.Email, result.Value.user.Email);
            Assert.IsTrue(user.IsEmailValidated);

            _userRepositoryMock?.Verify(repo => repo.Update(user), Times.Once);
        }
    }
}
