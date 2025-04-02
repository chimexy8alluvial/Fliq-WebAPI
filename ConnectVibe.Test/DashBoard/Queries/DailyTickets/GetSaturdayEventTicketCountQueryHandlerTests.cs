using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.DailyTicketCount;
using Fliq.Domain.Entities.Event;
using Moq;

[TestClass]
public class GetSaturdayEventTicketCountQueryHandlerTests
{
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private GetSaturdayEventTicketCountQueryHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetSaturdayEventTicketCountQueryHandler(_ticketRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidEventIdNoFilter_ReturnsCountResult()
    {
        int eventId = 1;
        int expectedCount = 5;
        var query = new GetSaturdayEventTicketCountQuery(eventId);

        _ticketRepositoryMock!
            .Setup(x => x.GetSaturdayTicketCountAsync(eventId, null))
            .ReturnsAsync(expectedCount);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(CountResult));
        Assert.AreEqual(expectedCount, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Fetching Saturday ticket count for EventId: {eventId}, TicketType: All"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Saturday Ticket Count for EventId {eventId}: {expectedCount}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetSaturdayTicketCountAsync(eventId, null), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ValidEventIdWithVipFilter_ReturnsCountResult()
    {
        int eventId = 2;
        int expectedCount = 3;
        var query = new GetSaturdayEventTicketCountQuery(eventId, TicketType.Vip);

        _ticketRepositoryMock!
            .Setup(x => x.GetSaturdayTicketCountAsync(eventId, TicketType.Vip))
            .ReturnsAsync(expectedCount);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(CountResult));
        Assert.AreEqual(expectedCount, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Fetching Saturday ticket count for EventId: {eventId}, TicketType: Vip"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Saturday Ticket Count for EventId {eventId}: {expectedCount}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetSaturdayTicketCountAsync(eventId, TicketType.Vip), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoSaturdayTickets_ReturnsZeroCountResult()
    {
        int eventId = 3;
        int expectedCount = 0;
        var query = new GetSaturdayEventTicketCountQuery(eventId);

        _ticketRepositoryMock!
            .Setup(x => x.GetSaturdayTicketCountAsync(eventId, null))
            .ReturnsAsync(expectedCount);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(CountResult));
        Assert.AreEqual(expectedCount, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Fetching Saturday ticket count for EventId: {eventId}, TicketType: All"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Saturday Ticket Count for EventId {eventId}: {expectedCount}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetSaturdayTicketCountAsync(eventId, null), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        int eventId = 4;
        var query = new GetSaturdayEventTicketCountQuery(eventId, TicketType.Regular);
        var exception = new Exception("Database connection failed");

        _ticketRepositoryMock!
            .Setup(x => x.GetSaturdayTicketCountAsync(eventId, TicketType.Regular))
            .ThrowsAsync(exception);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
        _loggerMock!.Setup(x => x.LogError(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual("GetSaturdayTicketCountFailed", result.FirstError.Code);
        Assert.IsTrue(result.FirstError.Description.Contains("Database connection failed"));

        _loggerMock.Verify(x => x.LogInfo($"Fetching Saturday ticket count for EventId: {eventId}, TicketType: Regular"), Times.Once());
        _loggerMock.Verify(x => x.LogError($"Error fetching Saturday ticket count for EventId {eventId}: {exception.Message}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetSaturdayTicketCountAsync(eventId, TicketType.Regular), Times.Once());
    }
}