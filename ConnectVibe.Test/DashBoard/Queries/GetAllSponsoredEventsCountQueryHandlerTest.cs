using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries.SponsoredEventsCount;
using Moq;

namespace Fliq.Test.DashBoard.Queries
{
    [TestClass]
    public class GetAllSponsoredEventsCountQueryHandlerTests
    {
        private Mock<IEventRepository>? _mockEventRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetAllSponsoredEventsCountQueryHandler? _handler;

        [TestInitialize]
        public void SetUp()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new GetAllSponsoredEventsCountQueryHandler(_mockEventRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsCorrectEventCount()
        {
            // Arrange
            int expectedCount = 100;
            _mockEventRepository?.Setup(repo => repo.CountAllSponsoredEvents()).ReturnsAsync(expectedCount);
            var query = new GetAllSponsoredEventsCountQuery();

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all sponsored-events count..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All sponsored-events count: {expectedCount}"), Times.Once);
            _mockEventRepository?.Verify(repo => repo.CountAllSponsoredEvents(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_LogsErrorAndReturnsError()
        {
            // Arrange
            _mockEventRepository?.Setup(repo => repo.CountAllSponsoredEvents()).ThrowsAsync(new Exception("Database error"));
            var query = new GetAllSponsoredEventsCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler?.Handle(query, CancellationToken.None)!;
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all sponsored-events count..."), Times.Once);
            _mockEventRepository?.Verify(repo => repo.CountAllSponsoredEvents(), Times.Once);
        }
    }
}
