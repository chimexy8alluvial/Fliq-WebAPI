using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries;
using Moq;

[TestClass]
public class GetEventTicketGrossRevenueQueryHandlerTests
{
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private GetEventTicketGrossRevenueQueryHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetEventTicketGrossRevenueQueryHandler(_ticketRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidEventId_ReturnsRevenue()
    {
        int eventId = 1;
        decimal expectedRevenue = 150.50m;
        var query = new GetEventTicketGrossRevenueQuery(eventId);

        _ticketRepositoryMock!
            .Setup(x => x.GetEventTicketGrossRevenueAsync(eventId))
            .ReturnsAsync(expectedRevenue);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(decimal));
        Assert.AreEqual(expectedRevenue, result.Value);

        _loggerMock.Verify(x => x.LogInfo($"Fetching gross revenue for EventId: {eventId}"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Gross revenue for EventId {eventId}: {expectedRevenue}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetEventTicketGrossRevenueAsync(eventId), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoTickets_ReturnsZero()
    {
        int eventId = 2;
        decimal expectedRevenue = 0m;
        var query = new GetEventTicketGrossRevenueQuery(eventId);

        _ticketRepositoryMock!
            .Setup(x => x.GetEventTicketGrossRevenueAsync(eventId))
            .ReturnsAsync(expectedRevenue);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(decimal));
        Assert.AreEqual(expectedRevenue, result.Value);

        _loggerMock.Verify(x => x.LogInfo($"Fetching gross revenue for EventId: {eventId}"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Gross revenue for EventId {eventId}: {expectedRevenue}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetEventTicketGrossRevenueAsync(eventId), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        int eventId = 3;
        var query = new GetEventTicketGrossRevenueQuery(eventId);
        var exception = new Exception("Database connection failed");

        _ticketRepositoryMock!
            .Setup(x => x.GetEventTicketGrossRevenueAsync(eventId))
            .ThrowsAsync(exception);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
        _loggerMock!.Setup(x => x.LogError(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual("GetGrossRevenueFailed", result.FirstError.Code);
        Assert.IsTrue(result.FirstError.Description.Contains("Database connection failed"));

        _loggerMock.Verify(x => x.LogInfo($"Fetching gross revenue for EventId: {eventId}"), Times.Once());
        _loggerMock.Verify(x => x.LogError($"Error fetching gross revenue for EventId {eventId}: {exception.Message}"), Times.Once());
        _ticketRepositoryMock.Verify(x => x.GetEventTicketGrossRevenueAsync(eventId), Times.Once());
    }
}