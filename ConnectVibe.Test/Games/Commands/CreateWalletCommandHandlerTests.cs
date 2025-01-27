using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Payments.Commands.CreateWallet;
using Fliq.Domain.Entities;
using Moq;
using Fliq.Domain.Common.Errors;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class CreateWalletCommandHandlerTests
    {
        private Mock<IWalletRepository> _walletRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ILoggerManager> _loggerMock;
        private CreateWalletCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILoggerManager>();

            _handler = new CreateWalletCommandHandler(
                _walletRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenUserNotFound()
        {
            // Arrange
            var command = new CreateWalletCommand(1);
            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
            _loggerMock.Verify(log => log.LogError("User not found"), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenWalletAlreadyExists()
        {
            // Arrange
            var command = new CreateWalletCommand(1);
            var user = new User { Id = 1, FirstName = "Test", LastName = "User" };
            var wallet = new Wallet { UserId = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns(user);
            _walletRepositoryMock.Setup(repo => repo.GetWalletByUserId(It.IsAny<int>())).Returns(wallet);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Wallet.AlreadyExists, result.FirstError);
            _loggerMock.Verify(log => log.LogError("Wallet already exists"), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldCreateWallet_WhenValidRequest()
        {
            // Arrange
            var command = new CreateWalletCommand(1);
            var user = new User { Id = 1, FirstName = "Test", LastName = "User" };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns(user);
            _walletRepositoryMock.Setup(repo => repo.GetWalletByUserId(It.IsAny<int>())).Returns((Wallet)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(command.UserId, result.Value.UserId);

            _walletRepositoryMock.Verify(repo => repo.Add(It.IsAny<Wallet>()), Times.Once);
            _loggerMock.Verify(log => log.LogInfo($"Creating wallet for user {command.UserId}"), Times.Once);
        }
    }
}