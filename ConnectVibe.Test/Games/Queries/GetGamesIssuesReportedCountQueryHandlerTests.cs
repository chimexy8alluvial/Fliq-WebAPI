
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Queries.GetIssuesReportedCount;
using Moq;

namespace Fliq.Test.Games.Queries
{
    [TestClass]
    public class GetGamesIssuesReportedCountQueryHandlerTests
    {
        private Mock<ILoggerManager>? _mockLogger;
        private Mock<IGamesRepository>? _mockGamesRepository;
        private GetGamesIssuesReportedCountQueryHandler _handler;
        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILoggerManager>();
            _mockGamesRepository = new Mock<IGamesRepository>();
            _handler = new GetGamesIssuesReportedCountQueryHandler(
                _mockGamesRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsGamesIssuesReportedCount()
        {
            //Arrange
            var gamesIssuesReportedCount = 5;
            _mockGamesRepository?.Setup(repo => repo.GetGamesIssuesReportedCountAsync()).ReturnsAsync(gamesIssuesReportedCount);

            var query = new GetGamesIssuesReportedCountQuery();

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(gamesIssuesReportedCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all issues reported games count ..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All issues reported games count: {gamesIssuesReportedCount}"), Times.Once);
            _mockGamesRepository?.Verify(repo => repo.GetGamesIssuesReportedCountAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_ThrowsException()
        {
            //Arrange
            _mockGamesRepository?.Setup(repo => repo.GetGamesIssuesReportedCountAsync()).ThrowsAsync(new Exception("Database error"));

            var query = new GetGamesIssuesReportedCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all issues reported games count ..."), Times.Once);
            _mockGamesRepository?.Verify(repo => repo.GetGamesIssuesReportedCountAsync(), Times.Once);
        }
    }
}
