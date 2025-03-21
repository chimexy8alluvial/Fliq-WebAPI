using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Queries.DatingDashboard;
using Fliq.Infrastructure.Persistence.Repositories;
using Moq;

namespace Fliq.Test.Dating.Query.SpeedDating
{
    [TestClass]
    public class GetSpeedDatingEventCountQueryHandlerTests
    {
        private Mock<ISpeedDatingEventRepository>? _mockSpeedDatingEventRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private SpeedDateCountQueryHandler? _handler;

        [TestInitialize]
        public void setup()
        {
            _mockSpeedDatingEventRepository = new Mock<ISpeedDatingEventRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new SpeedDateCountQueryHandler(_mockLogger.Object, _mockSpeedDatingEventRepository.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsSpeedDatingEventCount()
        {
            //Arrange
            var speedDatingCount = 5;
            _mockSpeedDatingEventRepository?.Setup(repo => repo.GetSpeedDateCountAsync()).ReturnsAsync(speedDatingCount);

            var query = new SpeedDateCountQuery();

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(speedDatingCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all speed date event count..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All speed date event count: {speedDatingCount}"), Times.Once);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetSpeedDateCountAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_ThrowsException()
        {
            //Arrange
            _mockSpeedDatingEventRepository?.Setup(repo => repo.GetSpeedDateCountAsync()).ThrowsAsync(new Exception("Database error"));

            var query = new SpeedDateCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all speed date event count..."), Times.Once);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetSpeedDateCountAsync(), Times.Once);
        }
    }
}



