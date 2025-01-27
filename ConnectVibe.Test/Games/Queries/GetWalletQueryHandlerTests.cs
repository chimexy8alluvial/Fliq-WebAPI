using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Payments.Queries.GetWallet;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Moq;

namespace Fliq.Test.Games.Queries
{
    [TestClass]
    public class GetWalletQueryHandlerTests
    {
        private Mock<IWalletRepository> _walletRepositoryMock;
        private GetWalletQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _handler = new GetWalletQueryHandler(_walletRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenWalletNotFound()
        {
            // Arrange
            var command = new GetWalletQuery(1);
            _walletRepositoryMock.Setup(repo => repo.GetWalletByUserId(It.IsAny<int>())).Returns((Wallet)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Wallet.NotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnWallet_WhenWalletFound()
        {
            // Arrange
            var command = new GetWalletQuery(1);
            var wallet = new Wallet { UserId = 1, Balance = 100m };
            _walletRepositoryMock.Setup(repo => repo.GetWalletByUserId(It.IsAny<int>())).Returns(wallet);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(wallet.UserId, result.Value.UserId);
            Assert.AreEqual(wallet.Balance, result.Value.Balance);
        }
    }
}