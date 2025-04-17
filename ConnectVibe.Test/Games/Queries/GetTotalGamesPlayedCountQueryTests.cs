
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Queries.GetTotalGamesPlayed;
using Moq;

namespace Fliq.Test.Games.Queries
{
    [TestClass]
    public class GetTotalGamesPlayedCountQueryTests
    {
        private Mock<IGamesRepository>? _mockGamesRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetTotalGamesPlayedCountQueryHandler _handler;
        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new GetTotalGamesPlayedCountQueryHandler(
                _mockLogger.Object,
                _mockGamesRepository.Object
            );
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsTotalGamesPlayedCount()
        {
            //Arrange
            var totalGamesPlayedCount = 5;
            _mockGamesRepository?.Setup(repo => repo.GetTotalGamesPlayedCountAsync()).ReturnsAsync(totalGamesPlayedCount);

            var query = new GetTotalGamesPlayedCountQuery();

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(totalGamesPlayedCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all total games played count ..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All total games played count: {totalGamesPlayedCount}"), Times.Once);
            _mockGamesRepository?.Verify(repo => repo.GetTotalGamesPlayedCountAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_ThrowsException()
        {
            //Arrange
            _mockGamesRepository?.Setup(repo => repo.GetTotalGamesPlayedCountAsync()).ThrowsAsync(new Exception("Database error"));

            var query = new GetTotalGamesPlayedCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all total games played count ..."), Times.Once);
            _mockGamesRepository?.Verify(repo => repo.GetTotalGamesPlayedCountAsync(), Times.Once);
        }
    }
}





