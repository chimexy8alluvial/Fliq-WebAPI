using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries.InActiveUserCount;
using Moq;

namespace Fliq.Test.DashBoard.Queries
{
    [TestClass]
    public class GetInActiveUsersCountQueryHandlerTests
    {
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetInActiveUsersCountQueryHandler? _handler;

        [TestInitialize]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new GetInActiveUsersCountQueryHandler(_mockUserRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsCorrectInactiveUsersCount()
        {
            // Arrange
            int expectedCount = 20;
            var query = new GetInActiveUsersCountQuery();

            _mockUserRepository?.Setup(repo => repo.CountInactiveUsers()).ReturnsAsync(expectedCount);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching Inactive users count..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"Inactive Users Count: {expectedCount}"), Times.Once);
            _mockUserRepository?.Verify(repo => repo.CountInactiveUsers(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_LogsErrorAndReturnsError()
        {
            // Arrange
            var query = new GetInActiveUsersCountQuery();

            _mockUserRepository?.Setup(repo => repo.CountInactiveUsers()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching Inactive users count..."), Times.Once);
            _mockUserRepository?.Verify(repo => repo.CountInactiveUsers(), Times.Once);
        }
    }
}