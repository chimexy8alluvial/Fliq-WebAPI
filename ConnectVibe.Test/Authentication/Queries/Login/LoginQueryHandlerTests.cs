using ConnectVibe.Application.Authentication.Queries.Login;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Security;
using Moq;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities;
namespace ConnectVibe.Test.Authentication.Queries.Login
{
    [TestClass]
    public class LoginQueryHandlerTests
    {
        private LoginQueryHandler _handler;
        private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _handler = new LoginQueryHandler(
                _jwtTokenGeneratorMock.Object,
                _userRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var query = new LoginQuery("johndoe@example.com", "password");

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(query.Email))
                .Returns((User)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidCredentials, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_InvalidPassword_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var query = new LoginQuery("johndoe@example.com", "wrongPassword");
            var user = new User { Email = query.Email, PasswordSalt = "salt", PasswordHash = PasswordHash.Create("correctPassword", "salt") };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(query.Email))
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Authentication.InvalidCredentials, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCredentials_GeneratesToken()
        {
            // Arrange
            var query = new LoginQuery("johndoe@example.com", "password");
            var user = new User { Email = query.Email, PasswordSalt = "salt", PasswordHash = PasswordHash.Create("password", "salt") };
            var expectedToken = "valid-token";

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(query.Email))
                .Returns(user);

            _jwtTokenGeneratorMock.Setup(generator => generator.GenerateToken(user))
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedToken, result.Value.Token);
            Assert.AreEqual(user, result.Value.user);
        }
    }
}
