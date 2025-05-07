using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Subscriptions.Commands;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Subscriptions;
using Moq;

namespace Fliq.Test.Subscriptions.Commands
{
    [TestClass]
    public class AddSubscriptionPlanPriceCommandHandlerTest
    {
        private Mock<ISubscriptionPlanRepository> _mockSubPlanRepo;
        private Mock<ISubscriptionPlanPriceRepository> _mockPriceRepo;
        private Mock<ILoggerManager> _mockLogger;
        private AddSubscriptionPlanPriceCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSubPlanRepo = new Mock<ISubscriptionPlanRepository>();
            _mockPriceRepo = new Mock<ISubscriptionPlanPriceRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new AddSubscriptionPlanPriceCommandHandler(_mockSubPlanRepo.Object, _mockLogger.Object, _mockPriceRepo.Object);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_AddsSubscriptionPlanPriceSuccessfully()
        {
            // Arrange
            var command = new AddSubscriptionPlanPriceCommand(1, 9.99m, "USD", "US", DateTime.UtcNow, "Web");

            var subPlan = new SubscriptionPlan { Id = 1, Name = "Basic" };

            _mockSubPlanRepo
                .Setup(repo => repo.GetByIdAsync(command.SubscriptionPlanId))
                .ReturnsAsync(subPlan);

            _mockPriceRepo
                .Setup(repo => repo.AddAsync(It.IsAny<SubscriptionPlanPrice>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _mockPriceRepo.Verify(repo => repo.AddAsync(It.IsAny<SubscriptionPlanPrice>()), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Successfully added new price"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_SubscriptionPlanNotFound_ReturnsError()
        {
            // Arrange
            var command = new AddSubscriptionPlanPriceCommand(99, 9.99m, "USD", "US", DateTime.UtcNow, "Web");

            _mockSubPlanRepo
                .Setup(repo => repo.GetByIdAsync(command.SubscriptionPlanId))
                .ReturnsAsync((SubscriptionPlan)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Subscription.DuplicateSubscriptionPlanPrice));
            _mockPriceRepo.Verify(repo => repo.AddAsync(It.IsAny<SubscriptionPlanPrice>()), Times.Never);
            _mockLogger.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("No subscription plan found"))), Times.Once);
        }
    }
}
