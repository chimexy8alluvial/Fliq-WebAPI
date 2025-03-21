using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries.UsersCount;
using Moq;

namespace Fliq.Test.DashBoard.Queries
{
    [TestClass]
    public class GetAllUsersCountQueryHandlerTests
    {
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetAllUsersCountQueryHandler? _handler;

        [TestInitialize]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new GetAllUsersCountQueryHandler(_mockUserRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsCorrectUserCount()
        {
            // Arrange
            int expectedCount = 100;
            _mockUserRepository!.Setup(repo => repo.CountAllUsers()).ReturnsAsync(expectedCount);
            _mockUserRepository?.Setup(repo => repo.CountAllUsers()).ReturnsAsync(expectedCount);
            var query = new GetAllUsersCountQuery();

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedCount, result.Value.Count);

            _mockLogger!.Verify(logger => logger.LogInfo("Fetching all users count..."), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo($"All Users Count: {expectedCount}"), Times.Once);
            _mockUserRepository.Verify(repo => repo.CountAllUsers(), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all users count..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All Users Count: {expectedCount}"), Times.Once);
            _mockUserRepository?.Verify(repo => repo.CountAllUsers(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_LogsErrorAndReturnsError()
        {
            // Arrange
            _mockUserRepository?.Setup(repo => repo.CountAllUsers()).ThrowsAsync(new Exception("Database error"));
            var query = new GetAllUsersCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler!.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all users count..."), Times.Once);
            _mockUserRepository?.Verify(repo => repo.CountAllUsers(), Times.Once);
        }
    }
}
