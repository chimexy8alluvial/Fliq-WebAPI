using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries.EventsCount;
using Moq;

namespace Fliq.Test.DashBoard.Queries
{
    [TestClass]
    public class GetAllEventsCountQueryHandlerTests
    {
        private Mock<IEventRepository>? _mockEventRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetAllEventsCountQueryHandler? _handler;

        [TestInitialize]
        public void SetUp()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new GetAllEventsCountQueryHandler(_mockEventRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WhenCalled_ReturnsCorrectEventCount()
        {
            // Arrange
            int expectedCount = 100;
            _mockEventRepository?.Setup(repo => repo.CountAllEvents()).ReturnsAsync(expectedCount);
            var query = new GetAllEventsCountQuery();

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedCount, result.Value.Count);

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all events count..."), Times.Once);
            _mockLogger?.Verify(logger => logger.LogInfo($"All events count: {expectedCount}"), Times.Once);
            _mockEventRepository?.Verify(repo => repo.CountAllEvents(), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryThrowsException_LogsErrorAndReturnsError()
        {
            // Arrange
            _mockEventRepository?.Setup(repo => repo.CountAllEvents()).ThrowsAsync(new Exception("Database error"));
            var query = new GetAllEventsCountQuery();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _handler?.Handle(query, CancellationToken.None)!;
            });

            _mockLogger?.Verify(logger => logger.LogInfo("Fetching all events count..."), Times.Once);
            _mockEventRepository?.Verify(repo => repo.CountAllEvents(), Times.Once);
        }
    }
}
