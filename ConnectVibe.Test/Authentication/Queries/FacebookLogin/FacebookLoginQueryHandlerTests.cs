﻿using ConnectVibe.Application.Authentication.Queries.FacebookLogin;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services.AuthServices;
using Moq;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities;
using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Services;

namespace ConnectVibe.Test.Authentication.Queries.FacebookLogin
{
    [TestClass]
    public class FacebookLoginQueryHandlerTests
    {
        private FacebookLoginQueryHandler _handler;
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

            _handler = new FacebookLoginQueryHandler(
                _jwtTokenGeneratorMock.Object,
                _userRepositoryMock.Object,
                _socialAuthServiceMock.Object,
                _loggerManagerMock.Object);
        }

        [TestMethod]
        public async Task Handle_InvalidFacebookToken_ReturnsInvalidTokenError()
        {
            // Arrange
            var query = new FacebookLoginQuery("invalid-code");

            _socialAuthServiceMock.Setup(service => service.GetFacebookUserInformation(query.Code))
                .ReturnsAsync((FacebookUserInfoResponse)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidToken, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidFacebookToken_UserExists_GeneratesToken()
        {
            // Arrange
            var query = new FacebookLoginQuery("valid-code");
            var facebookResponse = new FacebookUserInfoResponse
            {
                Email = "johndoe@example.com",
                FirstName = "John",
                LastName = "Doe",
                Name = "John Doe"
            };
            var user = new User { Email = facebookResponse.Email };
            var expectedToken = "valid-token";

            _socialAuthServiceMock.Setup(service => service.GetFacebookUserInformation(query.Code))
                .ReturnsAsync(facebookResponse);

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(facebookResponse.Email))
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
        public async Task Handle_ValidFacebookToken_NewUser_CreatesUserAndGeneratesToken()
        {
            // Arrange
            var query = new FacebookLoginQuery("valid-code");
            var facebookResponse = new FacebookUserInfoResponse
            {
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Name = "New User"
            };
            var expectedToken = "valid-token";

            _socialAuthServiceMock.Setup(service => service.GetFacebookUserInformation(query.Code))
                .ReturnsAsync(facebookResponse);

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(facebookResponse.Email))
                .Returns((User)null);

            _jwtTokenGeneratorMock.Setup(generator => generator.GenerateToken(It.IsAny<User>()))
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedToken, result.Value.Token);
            Assert.AreEqual(facebookResponse.Email, result.Value.user.Email);
            Assert.IsTrue(result.Value.IsNewUser);

            _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
        }
    }
}
