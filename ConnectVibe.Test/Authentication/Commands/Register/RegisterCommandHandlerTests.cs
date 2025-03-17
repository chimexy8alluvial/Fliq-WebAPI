using Fliq.Application.Authentication.Commands.Register;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using MapsterMapper;
using Moq;

namespace Fliq.Test.Authentication.Commands.Register
{
    [TestClass]
    public class RegisterCommandHandlerTests
    {
        private RegisterCommandHandler _handler;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IOtpService> _otpServiceMock;
        private Mock<ILoggerManager> _loggerManagerMock;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _otpServiceMock = new Mock<IOtpService>();
            _loggerManagerMock = new Mock<ILoggerManager>();

            _handler = new RegisterCommandHandler(
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _emailServiceMock.Object,
                _otpServiceMock.Object,
                _loggerManagerMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserAlreadyExists_ReturnsDuplicateEmailError()
        {
            // Arrange
            var command = new RegisterCommand("John", "Doe", "johndoe", "johndoe@example.com", "password", "English");
            var existingUser = new User { Email = command.Email, IsEmailValidated = true };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.DuplicateEmail, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_NewUser_CreatesUserAndSendsOtp()
        {
            // Arrange
            var command = new RegisterCommand("John", "Doe", "johndoe", "johndoe@example.com", "password", "English");
            User newUser = null;

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns((User)null);

            _mapperMock.Setup(mapper => mapper.Map<User>(command))
                .Returns(new User { Email = command.Email });

            _otpServiceMock.Setup(service => service.GetOtpAsync(command.Email, It.IsAny<int>()))
                .ReturnsAsync("123456");

            _emailServiceMock.Setup(service => service.SendEmailAsync(command.Email, "Your OTP Code", "Your OTP is 123456"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual("123456", result.Value.otp);

            _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
            _emailServiceMock.Verify(service => service.SendEmailAsync(command.Email, "Your OTP Code", "Your OTP is 123456"), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ExistingUserButEmailNotValidated_CreatesUserAndSendsOtp()
        {
            // Arrange
            var command = new RegisterCommand("John", "Doe", "johndoe", "johndoe@example.com", "password", "English");
            var existingUser = new User { Email = command.Email, IsEmailValidated = false };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(command.Email))
                .Returns(existingUser);

            _otpServiceMock.Setup(service => service.GetOtpAsync(command.Email, It.IsAny<int>()))
                .ReturnsAsync("123456");

            _emailServiceMock.Setup(service => service.SendEmailAsync(command.Email, "Your OTP Code", "Your OTP is 123456"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual("123456", result.Value.otp);

            _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
            _emailServiceMock.Verify(service => service.SendEmailAsync(command.Email, "Your OTP Code", "Your OTP is 123456"), Times.Once);
        }
    }
}