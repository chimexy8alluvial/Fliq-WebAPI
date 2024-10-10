using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Authentication.Queries.GoogleLogin;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Interfaces.Services.AuthServices;
using ConnectVibe.Domain.Entities;
using Moq;

namespace ConnectVibe.Test.Authentication.Queries.GoogleLogin
{
    [TestClass]
    public class GoogleLoginQueryHandlerTests
    {
        private GoogleLoginQueryHandler _handler;
        private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ISocialAuthService> _socialAuthServiceMock;
        private Mock<ILoggerManager> _loggerManagerMock;

        [TestInitialize]
        public void Setup()
        {
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _socialAuthServiceMock = new Mock<ISocialAuthService>();
            _loggerManagerMock = new Mock<ILoggerManager>();

            _handler = new GoogleLoginQueryHandler(
                _jwtTokenGeneratorMock.Object,
                _userRepositoryMock.Object,
                _socialAuthServiceMock.Object,
                _loggerManagerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ValidGoogleCode_UserExists_GeneratesToken()
        {
            // Arrange
            var query = new GoogleLoginQuery("valid-code");
            var googleResponse = new GooglePayloadResponse
            {
                Email = "johndoe@example.com",
                GivenName = "John",
                FamilyName = "Doe",
                Name = "John Doe",
                EmailVerified = true
            };
            var user = new User { Email = googleResponse.Email };
            var expectedToken = "valid-token";

            _socialAuthServiceMock.Setup(service => service.ExchangeCodeForTokenAsync(query.Code))
                .ReturnsAsync(googleResponse);

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(googleResponse.Email))
                .Returns(user);

            _jwtTokenGeneratorMock.Setup(generator => generator.GenerateToken(user))
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedToken, result.Value.Token);
            Assert.AreEqual(user, result.Value.user);
            Assert.IsFalse(result.Value.IsNewUser);
        }

        [TestMethod]
        public async Task Handle_ValidGoogleCode_NewUser_CreatesUserAndGeneratesToken()
        {
            // Arrange
            var query = new GoogleLoginQuery("valid-code");
            var googleResponse = new GooglePayloadResponse
            {
                Email = "newuser@example.com",
                GivenName = "New",
                FamilyName = "User",
                Name = "New User",
                EmailVerified = true
            };
            var expectedToken = "valid-token";

            _socialAuthServiceMock.Setup(service => service.ExchangeCodeForTokenAsync(query.Code))
                .ReturnsAsync(googleResponse);

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(googleResponse.Email))
                .Returns((User)null);

            _jwtTokenGeneratorMock.Setup(generator => generator.GenerateToken(It.IsAny<User>()))
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedToken, result.Value.Token);
            Assert.AreEqual(googleResponse.Email, result.Value.user.Email);
            Assert.IsTrue(result.Value.IsNewUser);

            _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
        }
    }
}
