using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.Common.Interfaces.Services.SubscriptionServices;
using Fliq.Application.Payments.Commands.RevenueCat;
using Fliq.Application.Payments.Common;
using Fliq.Domain.Entities;
using MapsterMapper;
using Moq;

namespace Fliq.Test
{
    [TestClass]
    public class RevenueCatWebhookHandlerTests
    {
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<ISubscriptionService>? _subscriptionServiceMock;
        private Mock<IRevenueCatServices>? _revenueCatServicesMock;
        private Mock<IMapper>? _mapperMock;
        private Mock<ILoggerManager>? _loggerMock;
        private RevenueCatWebhookHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _revenueCatServicesMock = new Mock<IRevenueCatServices>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new RevenueCatWebhookHandler(
                _userRepositoryMock.Object,
                _subscriptionServiceMock.Object,
                _revenueCatServicesMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_RefundedEvent_ProcessesRefund()
        {
            // Arrange
            var command = new RevenueCatWebhookCommand
            {
                Event = new RevenueCatWebhookCommand.EventInfo
                {
                    Type = "REFUNDED",
                    AppUserId = "2",
                    TransactionId = "txn_100"
                }
            };
            var user = new User { Id = 2 };
            var payload = new RevenueCatWebhookPayload { Event = new RevenueCatWebhookPayload.EventInfo { TransactionId = "txn_100" } };

            _userRepositoryMock!.Setup(r => r.GetUserById(2)).Returns(user);
            _mapperMock!.Setup(m => m.Map<RevenueCatWebhookPayload>(command)).Returns(payload);
            _revenueCatServicesMock!.Setup(r => r.RefundTransactionAsync("txn_100")).ReturnsAsync(true);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsTrue(result.Value.Success);
            Assert.AreEqual("Operation completed successfully.", result.Value.Message);
            _revenueCatServicesMock.Verify(r => r.RefundTransactionAsync("txn_100"), Times.Once());
            _loggerMock!.Verify(l => l.LogError(It.IsAny<string>()), Times.Never());
        }
    }
}
