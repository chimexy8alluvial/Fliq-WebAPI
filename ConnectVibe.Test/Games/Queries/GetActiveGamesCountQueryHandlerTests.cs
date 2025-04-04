
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Moq;
using static Fliq.Application.Games.Queries.GetActiveGamesCountQuery.GetActiveGamesCountQuery;

namespace Fliq.Test.Games.Queries
{
    [TestClass]
    public class GetActiveGamesCountQueryHandlerTests
    {
        private Mock<IGamesRepository>? _mockGamesRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private ActiveGamesCountQueryHandler _handler;
        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new ActiveGamesCountQueryHandler(
                _mockLogger.Object,
                _mockGamesRepository.Object
            );
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsActiveGamesCount()
        {
            //Arrange
            var activeGamesCount = 5;
            _mockGamesRepository?.Setup(repo => repo.GetActiveGamesCountAsync()).ReturnsAsync(activeGamesCount);

            var query = new ActiveGamesCountQuery();

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(activeGamesCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all active games count ..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All active games count: {activeGamesCount}"), Times.Once);
            _mockGamesRepository?.Verify(repo => repo.GetActiveGamesCountAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_ThrowsException()
        {
            //Arrange
            _mockGamesRepository?.Setup(repo => repo.GetActiveGamesCountAsync()).ThrowsAsync(new Exception("Database error"));

            var query = new ActiveGamesCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all active games count ..."), Times.Once);
            _mockGamesRepository?.Verify(repo => repo.GetActiveGamesCountAsync(), Times.Once);
        }
    }
}
