using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Queries.DatingDashboard.BlindDte;
using Moq;

namespace Fliq.Test.Dating.Query.BlindDating
{
    [TestClass]
    public class GetBlindDateEventCountQueryHndlerTests
    {
        private Mock<IBlindDateRepository>? _mockBlindDateRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private BlindDateCountQueryHandler? _handler;
        [TestInitialize]
        public void Setup()
        {
            _mockBlindDateRepository = new Mock<IBlindDateRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new BlindDateCountQueryHandler(_mockLogger.Object, _mockBlindDateRepository.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsBlindDateEventCount()
        {
            //Arrange
            var blindDateCount = 5;
            _mockBlindDateRepository?.Setup(repo => repo.GetBlindDateCountAsync()).ReturnsAsync(blindDateCount);

            var query = new BlindDateCountQuery();

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(blindDateCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all blind date count..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All blind date count: {blindDateCount}"), Times.Once);
            _mockBlindDateRepository?.Verify(repo => repo.GetBlindDateCountAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_ThrowsException()
        {
            //Arrange
            _mockBlindDateRepository?.Setup(repo => repo.GetBlindDateCountAsync()).ThrowsAsync(new Exception("Database error"));

            var query = new BlindDateCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all blind date count..."), Times.Once);
            _mockBlindDateRepository?.Verify(repo => repo.GetBlindDateCountAsync(), Times.Once);
        }
    }
}

