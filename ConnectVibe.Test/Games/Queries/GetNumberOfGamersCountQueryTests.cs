

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Queries.GetNumberOfGamersCountQuery;
using Moq;

namespace Fliq.Test.Games.Queries
{
    [TestClass]
    public class GetNumberOfGamersCountQueryTests
    {
        private Mock<IGamesRepository>? _mockGamesRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetNumberOfGamersCountQueryHandler _handler;
        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new GetNumberOfGamersCountQueryHandler(
                _mockLogger.Object,
                _mockGamesRepository.Object
            );
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsNumberOfGamersCount()
        {
            //Arrange
            var totalGamersCount = 5;
            _mockGamesRepository?.Setup(repo => repo.GetGamersCountAsync()).ReturnsAsync(totalGamersCount);

            var query = new GetNumberOfGamersCountQuery();

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(totalGamersCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all gamers count ..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All gamers count: {totalGamersCount}"), Times.Once);
            _mockGamesRepository?.Verify(repo => repo.GetGamersCountAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_ThrowsException()
        {
            //Arrange
            _mockGamesRepository?.Setup(repo => repo.GetGamersCountAsync()).ThrowsAsync(new Exception("Database error"));

            var query = new GetNumberOfGamersCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler.Handle(query, CancellationToken.None);
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all gamers count ..."), Times.Once);
            _mockGamesRepository?.Verify(repo => repo.GetGamersCountAsync(), Times.Once);
        }
    }
}
