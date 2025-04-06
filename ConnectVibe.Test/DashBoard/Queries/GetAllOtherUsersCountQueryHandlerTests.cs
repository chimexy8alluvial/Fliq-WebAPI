using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries.OtherUsersCount;
using Moq;

namespace Fliq.Test.DashBoard.Queries
{
    [TestClass]
    public class GetAllOtherUsersCountQueryHandlerTests
    {
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetAllOtherUsersCountQueryHandler? _handler;

        [TestInitialize]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new GetAllOtherUsersCountQueryHandler(_mockUserRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsCorrectUserCount()
        {
            // Arrange
            int expectedCount = 100;
            _mockUserRepository?.Setup(repo => repo.CountAllOtherUsers()).ReturnsAsync(expectedCount);
            var query = new GetAllOtherUsersCountQuery();

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all other-users count..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All other-users count: {expectedCount}"), Times.Once);
            _mockUserRepository?.Verify(repo => repo.CountAllOtherUsers(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_LogsErrorAndReturnsError()
        {
            // Arrange
            _mockUserRepository?.Setup(repo => repo.CountAllOtherUsers()).ThrowsAsync(new Exception("Database error"));
            var query = new GetAllOtherUsersCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler?.Handle(query, CancellationToken.None)!;
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all other-users count..."), Times.Once);
            _mockUserRepository?.Verify(repo => repo.CountAllOtherUsers(), Times.Once);
        }
    }
}
