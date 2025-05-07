

using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Subscriptions.Commands;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Subscriptions;
using Moq;

namespace Fliq.Test.Subscriptions.Commands
{
    [TestClass]
    public class CreateSubscriptionPlanCommandHandlerTest
    {
        private Mock<ISubscriptionPlanRepository> _mockSubPlanRepo;
        private Mock<ILoggerManager> _mockLogger;
        private CreateSubscriptionPlanCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSubPlanRepo = new Mock<ISubscriptionPlanRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new CreateSubscriptionPlanCommandHandler(_mockSubPlanRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_AddsSubscriptionPlanSuccessfully()
        {
            // Arrange
            var command = new CreateSubscriptionPlanCommand("Basic", "prod_001", "Basic plan");

            _mockSubPlanRepo
                .Setup(repo => repo.GetByName(command.Name))
                .ReturnsAsync((SubscriptionPlan)null); // No duplicate

            _mockSubPlanRepo
                .Setup(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _mockSubPlanRepo.Verify(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>()), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Successfully added new subscription plan"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_DuplicateName_ReturnsError()
        {
            // Arrange
            var command = new CreateSubscriptionPlanCommand("Basic", "prod_001", "Duplicate");

            _mockSubPlanRepo
                .Setup(repo => repo.GetByName(command.Name))
                .ReturnsAsync(new SubscriptionPlan());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Subscription.DuplicateSubscriptionPlan));
            _mockSubPlanRepo.Verify(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>()), Times.Never);
            _mockLogger.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("Duplicate subscription plan detected"))), Times.Once);
        }
    }
}
