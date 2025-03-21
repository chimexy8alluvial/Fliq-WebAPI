using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries.ActiveUserCount;
using Moq;

namespace Fliq.Test.DashBoard.Queries
{
    [TestClass]
    public class GetActiveUsersCountQueryHandlerTests
    {
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetActiveUsersCountQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new GetActiveUsersCountQueryHandler(_mockUserRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsActiveUsersCount()
        {
            // Arrange
            var activeUserCount = 10;
            _mockUserRepository?.Setup(repo => repo.CountActiveUsers()).ReturnsAsync(activeUserCount);

            var query = new GetActiveUsersCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(activeUserCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching active users count..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"Active Users Count: {activeUserCount}"), Times.Once);
            _mockUserRepository?.Verify(repo => repo.CountActiveUsers(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_ThrowsException()
        {
            // Arrange
            _mockUserRepository?
                .Setup(repo => repo.CountActiveUsers())
                .ThrowsAsync(new Exception("Database error"));

            var query = new GetActiveUsersCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching active users count..."), Times.Once);
            _mockUserRepository?.Verify(repo => repo.CountActiveUsers(), Times.Once);
        }
    }
}
