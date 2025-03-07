

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands.BlindDateCategory;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.DatingEnvironment;
using Moq;

namespace Fliq.Test.Dating.Commands
{
    [TestClass]
    public class AddBlindDateCategoryCommandHandlerTest
    {
        private Mock<IBlindDateCategoryRepository> _mockBlindDateCategoryRepository;
        private Mock<ILoggerManager> _mockLoggerManager;
        private AddBlindDateCategoryCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockBlindDateCategoryRepository = new Mock<IBlindDateCategoryRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();

            _handler = new AddBlindDateCategoryCommandHandler(
                _mockBlindDateCategoryRepository.Object,
                _mockLoggerManager.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_AddsCategorySuccessfully()
        {
            // Arrange
            var command = new AddBlindDateCategoryCommand("Romantic", "For couples looking for love");

            _mockBlindDateCategoryRepository
                .Setup(repo => repo.GetByCategoryName(command.CategoryName))
                .ReturnsAsync((BlindDateCategory)null); // Category does not exist

            _mockBlindDateCategoryRepository
                .Setup(repo => repo.AddAsync(It.IsAny<BlindDateCategory>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(command.CategoryName, result.Value.CategoryName);

            _mockBlindDateCategoryRepository.Verify(repo => repo.AddAsync(It.IsAny<BlindDateCategory>()), Times.Once);
            _mockLoggerManager.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Successfully added new blind date category"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_DuplicateCategory_ReturnsError()
        {
            // Arrange
            var command = new AddBlindDateCategoryCommand("Romantic", "For couples looking for love");

            var existingCategory = new BlindDateCategory { Id = 1, CategoryName = "Romantic", Description = "Existing category" };

            _mockBlindDateCategoryRepository
                .Setup(repo => repo.GetByCategoryName(command.CategoryName))
                .ReturnsAsync(existingCategory); // Category already exists

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Dating.DuplicateBlindDateCategory));

            _mockBlindDateCategoryRepository.Verify(repo => repo.AddAsync(It.IsAny<BlindDateCategory>()), Times.Never);
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("Duplicate blind date category detected"))), Times.Once);
        }

    }
}
