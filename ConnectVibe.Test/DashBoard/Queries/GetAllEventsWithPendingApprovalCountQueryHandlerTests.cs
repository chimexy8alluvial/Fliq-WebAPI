using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.EventsWithPendingApproval;
using Moq;

namespace Fliq.Application.Tests.DashBoard.Queries
{
    [TestClass]
    public class GetAllEventsWithPendingApprovalCountQueryHandlerTests
    {
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetAllEventsWithPendingApprovalCountQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllEventsWithPendingApprovalCountQueryHandler(_eventRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_SuccessfulCount_ReturnsCountResult()
        {
            // Arrange
            var query = new GetAllEventsWithPendingApprovalCountQuery();
            int expectedCount = 5;

            _eventRepositoryMock!
                .Setup(repo => repo.CountAllEventsWithPendingApproval())
                .ReturnsAsync(expectedCount);

            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            
            Assert.IsInstanceOfType(result.Value, typeof(CountResult));
            Assert.AreEqual(expectedCount, result.Value.Count);

            _loggerMock.Verify(logger => logger.LogInfo("Fetching all events with pending approval count..."), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo($"All events with pending approval count: {expectedCount}"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ZeroCount_ReturnsZeroCountResult()
        {
            // Arrange
            var query = new GetAllEventsWithPendingApprovalCountQuery();
            int expectedCount = 0;

            _eventRepositoryMock!
                .Setup(repo => repo.CountAllEventsWithPendingApproval())
                .ReturnsAsync(expectedCount);

            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
           
            Assert.IsInstanceOfType(result.Value, typeof(CountResult));
            Assert.AreEqual(expectedCount, result.Value.Count);

            _loggerMock.Verify(logger => logger.LogInfo("Fetching all events with pending approval count..."), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo($"All events with pending approval count: {expectedCount}"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var query = new GetAllEventsWithPendingApprovalCountQuery();

            _eventRepositoryMock!
                .Setup(repo => repo.CountAllEventsWithPendingApproval())
                .ThrowsAsync(new Exception("Database connection failed"));

            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
                await _handler!.Handle(query, CancellationToken.None),
                "Database connection failed");

            _loggerMock.Verify(logger => logger.LogInfo("Fetching all events with pending approval count..."), Times.Once());
           
        }
    }

    
  
}