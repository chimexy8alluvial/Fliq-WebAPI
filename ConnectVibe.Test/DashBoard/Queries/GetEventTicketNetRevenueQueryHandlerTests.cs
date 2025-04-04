using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries;
using Moq;

[TestClass]
public class GetEventTicketNetRevenueQueryHandlerTests
{
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private GetEventTicketNetRevenueQueryHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetEventTicketNetRevenueQueryHandler(_ticketRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidEventId_ReturnsNetRevenue()
    {
        // Arrange
        int eventId = 1;
        decimal expectedRevenue = 450.00m; // Example: $500 gross - $50 refunded
        var query = new GetEventTicketNetRevenueQuery(eventId);

        _ticketRepositoryMock!
            .Setup(repo => repo.GetEventTicketNetRevenueAsync(eventId))
            .ReturnsAsync(expectedRevenue);

        _loggerMock!.Setup(l => l.LogInfo(It.IsAny<string>())).Verifiable();

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        
        Assert.AreEqual(expectedRevenue, result.Value);
        _loggerMock.Verify(l => l.LogInfo($"Fetching net revenue for EventId: {eventId}"), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Net revenue for EventId {eventId}: {expectedRevenue}"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        var query = new GetEventTicketNetRevenueQuery(eventId);
        var exception = new Exception("Database error");

        _ticketRepositoryMock!
            .Setup(repo => repo.GetEventTicketNetRevenueAsync(eventId))
            .ThrowsAsync(exception);

        _loggerMock!.Setup(l => l.LogInfo(It.IsAny<string>())).Verifiable();
        _loggerMock.Setup(l => l.LogError(It.IsAny<string>())).Verifiable();

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("GetNetRevenueFailed", result.FirstError.Code);
        Assert.AreEqual($"Failed to fetch net revenue: {exception.Message}", result.FirstError.Description);
        _loggerMock.Verify(l => l.LogError($"Error fetching net revenue for EventId {eventId}: {exception.Message}"), Times.Once());
    }
}