
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries.NewSignUpsCount;
using Moq;

namespace Fliq.Test.DashBoard.Queries
{
    [TestClass]
    public class GetNewSignUpsCountQueryHandlerTests
    {

        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILoggerManager> _mockLogger;
        private GetNewSignUpsCountQueryHandler _handler;

        [TestInitialize]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new GetNewSignUpsCountQueryHandler(_mockUserRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsCorrectNewSignUpsCount()
        {
            // Arrange
            int days = 7;
            int expectedCount = 50;
            var query = new GetNewSignUpsCountQuery(days);

            _mockUserRepository.Setup(repo => repo.CountNewSignups(days)).ReturnsAsync(expectedCount);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedCount, result.Value.Count);

            _mockLogger.Verify(logger => logger.LogInfo($"Fetching new signups count in the last {days} days..."), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo($"New Signups Count: {expectedCount}"), Times.Once);
            _mockUserRepository.Verify(repo => repo.CountNewSignups(days), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_LogsErrorAndReturnsError()
        {
            // Arrange
            int days = 7;
            var query = new GetNewSignUpsCountQuery(days);

            _mockUserRepository.Setup(repo => repo.CountNewSignups(days)).ThrowsAsync(new Exception("Database error"));

             // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger.Verify(logger => logger.LogInfo($"Fetching new signups count in the last {days} days..."), Times.Once);
            _mockUserRepository.Verify(repo => repo.CountNewSignups(days), Times.Once);
        }
    }
}
