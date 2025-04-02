using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.VipTicketCount;
using Moq;

[TestClass]
public class GetEventVipTicketCountQueryHandlerTests
{
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private GetEventVipTicketCountQueryHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetEventVipTicketCountQueryHandler(_ticketRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidEventId_ReturnsCountResult()
    {
        // Arrange
        int eventId = 1;
        int expectedCount = 3;
        var query = new GetEventVipTicketCountQuery(eventId);

        _ticketRepositoryMock!
            .Setup(x => x.GetVipTicketCountAsync(eventId))
            .ReturnsAsync(expectedCount);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(CountResult));
        Assert.AreEqual(expectedCount, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Fetching VIP ticket count for EventId: {eventId}"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"VIP Ticket Count for EventId {eventId}: {expectedCount}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetVipTicketCountAsync(eventId), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoVipTickets_ReturnsZeroCountResult()
    {
        // Arrange
        int eventId = 2;
        int expectedCount = 0;
        var query = new GetEventVipTicketCountQuery(eventId);

        _ticketRepositoryMock!
            .Setup(x => x.GetVipTicketCountAsync(eventId))
            .ReturnsAsync(expectedCount);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(CountResult));
        Assert.AreEqual(expectedCount, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Fetching VIP ticket count for EventId: {eventId}"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"VIP Ticket Count for EventId {eventId}: {expectedCount}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetVipTicketCountAsync(eventId), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        // Arrange
        int eventId = 3;
        var query = new GetEventVipTicketCountQuery(eventId);
        var exception = new Exception("Database connection failed");

        _ticketRepositoryMock!
            .Setup(x => x.GetVipTicketCountAsync(eventId))
            .ThrowsAsync(exception);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(ErrorType.Failure, result.FirstError.Type);
        Assert.IsTrue(result.FirstError.Description.Contains("Database connection failed"));

        _loggerMock.Verify(x => x.LogInfo($"Fetching VIP ticket count for EventId: {eventId}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetVipTicketCountAsync(eventId), Times.Once());
    }
}