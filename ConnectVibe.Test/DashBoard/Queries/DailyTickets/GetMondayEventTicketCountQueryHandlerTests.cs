using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.DailyTicketCount;
using Fliq.Application.DashBoard.Queries.GetAllEvents;
using Fliq.Domain.Entities.Event;
using Moq;

[TestClass]
public class GetMondayEventTicketCountQueryHandlerTests
{
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private GetMondayEventTicketCountQueryHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetMondayEventTicketCountQueryHandler(_ticketRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidEventIdNoFilter_ReturnsCountResult()
    {
        // Arrange
        int eventId = 1;
        int expectedCount = 5; // Total Monday tickets
        var query = new GetMondayEventTicketCountQuery(eventId);

        _ticketRepositoryMock!
            .Setup(x => x.GetMondayTicketCountAsync(eventId, null))
            .ReturnsAsync(expectedCount);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(CountResult));
        Assert.AreEqual(expectedCount, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Fetching Monday ticket count for EventId: {eventId}, TicketType: All"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Monday Ticket Count for EventId {eventId}: {expectedCount}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetMondayTicketCountAsync(eventId, null), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ValidEventIdWithVipFilter_ReturnsCountResult()
    {
        // Arrange
        int eventId = 2;
        int expectedCount = 3; // VIP Monday tickets
        var query = new GetMondayEventTicketCountQuery(eventId, TicketType.Vip);

        _ticketRepositoryMock!
            .Setup(x => x.GetMondayTicketCountAsync(eventId, TicketType.Vip))
            .ReturnsAsync(expectedCount);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(CountResult));
        Assert.AreEqual(expectedCount, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Fetching Monday ticket count for EventId: {eventId}, TicketType: Vip"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Monday Ticket Count for EventId {eventId}: {expectedCount}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetMondayTicketCountAsync(eventId, TicketType.Vip), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoMondayTickets_ReturnsZeroCountResult()
    {
        // Arrange
        int eventId = 3;
        int expectedCount = 0;
        var query = new GetMondayEventTicketCountQuery(eventId);

        _ticketRepositoryMock!
            .Setup(x => x.GetMondayTicketCountAsync(eventId, null))
            .ReturnsAsync(expectedCount);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(CountResult));
        Assert.AreEqual(expectedCount, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Fetching Monday ticket count for EventId: {eventId}, TicketType: All"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Monday Ticket Count for EventId {eventId}: {expectedCount}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetMondayTicketCountAsync(eventId, null), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        // Arrange
        int eventId = 4;
        var query = new GetMondayEventTicketCountQuery(eventId, TicketType.Regular);
        var exception = new Exception("Database connection failed");

        _ticketRepositoryMock!
            .Setup(x => x.GetMondayTicketCountAsync(eventId, TicketType.Regular))
            .ThrowsAsync(exception);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(ErrorType.Failure, result.FirstError.Type);
        Assert.IsTrue(result.FirstError.Description.Contains("Database connection failed"));

        _loggerMock.Verify(x => x.LogInfo($"Fetching Monday ticket count for EventId: {eventId}, TicketType: Regular"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetMondayTicketCountAsync(eventId, TicketType.Regular), Times.Once());
    }
}
