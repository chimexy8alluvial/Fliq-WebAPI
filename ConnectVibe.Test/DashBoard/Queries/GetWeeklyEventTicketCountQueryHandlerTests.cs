using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.DailyTicketCount;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using Moq;

[TestClass]
public class GetWeeklyEventTicketCountQueryHandlerTests
{
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<IEventRepository>? _eventRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private GetWeeklyEventTicketCountQueryHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetWeeklyEventTicketCountQueryHandler(_ticketRepositoryMock.Object, _loggerMock.Object, _eventRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidEventIdAndDateRange_ReturnsWeeklyCountResult()
    {
        // Arrange
        int eventId = 1;
        DateTime startDate = new DateTime(2025, 4, 1); // Tuesday
        DateTime endDate = new DateTime(2025, 4, 7);   // Monday
        var query = new GetWeeklyEventTicketCountQuery(eventId, startDate, endDate);

        var expectedCounts = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Sunday, 0 },
            { DayOfWeek.Monday, 0 },
            { DayOfWeek.Tuesday, 10 },
            { DayOfWeek.Wednesday, 15 },
            { DayOfWeek.Thursday, 0 },
            { DayOfWeek.Friday, 0 },
            { DayOfWeek.Saturday, 0 }
        };

        _eventRepositoryMock!
            .Setup(x => x.GetEventById(eventId))
            .Returns(new Events());

        _ticketRepositoryMock!
            .Setup(x => x.GetWeeklyTicketCountAsync(eventId, startDate, endDate, null))
            .ReturnsAsync(expectedCounts);

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(WeeklyCountResult));
        CollectionAssert.AreEqual(
            expectedCounts.OrderBy(kv => kv.Key).ToList(),
            result.Value.DailyCounts.OrderBy(kv => kv.Key).ToList());

        _loggerMock!.Verify(
            x => x.LogInfo($"Weekly Ticket Counts for EventId {eventId}: Sunday: 0, Monday: 0, Tuesday: 10, Wednesday: 15, Thursday: 0, Friday: 0, Saturday: 0"),
            Times.Once());
        _ticketRepositoryMock!.Verify(
            x => x.GetWeeklyTicketCountAsync(eventId, startDate, endDate, null),
            Times.Once());
        _eventRepositoryMock!.Verify(
            x => x.GetEventById(eventId),
            Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoTicketsInDateRange_ReturnsFullWeekWithZeros()
    {
        // Arrange
        int eventId = 2;
        DateTime startDate = new DateTime(2025, 4, 1);
        DateTime endDate = new DateTime(2025, 4, 7);
        var query = new GetWeeklyEventTicketCountQuery(eventId, startDate, endDate);

        var expectedCounts = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Sunday, 0 },
            { DayOfWeek.Monday, 0 },
            { DayOfWeek.Tuesday, 0 },
            { DayOfWeek.Wednesday, 0 },
            { DayOfWeek.Thursday, 0 },
            { DayOfWeek.Friday, 0 },
            { DayOfWeek.Saturday, 0 }
        };

        _eventRepositoryMock!
            .Setup(x => x.GetEventById(eventId))
            .Returns(new Events());

        _ticketRepositoryMock!
            .Setup(x => x.GetWeeklyTicketCountAsync(eventId, startDate, endDate, null))
            .ReturnsAsync(expectedCounts);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(WeeklyCountResult));
        CollectionAssert.AreEqual(
            expectedCounts.OrderBy(kv => kv.Key).ToList(),
            result.Value.DailyCounts.OrderBy(kv => kv.Key).ToList());

        _loggerMock.Verify(
            x => x.LogInfo($"Weekly Ticket Counts for EventId {eventId}: Sunday: 0, Monday: 0, Tuesday: 0, Wednesday: 0, Thursday: 0, Friday: 0, Saturday: 0"),
            Times.Once());
        _ticketRepositoryMock.Verify(
            x => x.GetWeeklyTicketCountAsync(eventId, startDate, endDate, null),
            Times.Once());
        _eventRepositoryMock.Verify(
            x => x.GetEventById(eventId),
            Times.Once());
    }

    [TestMethod]
    public async Task Handle_InvalidEventId_ReturnsEventNotFoundError()
    {
        // Arrange
        int eventId = 3;
        var query = new GetWeeklyEventTicketCountQuery(eventId, null, null);

        _eventRepositoryMock!
            .Setup(x => x.GetEventById(eventId))
            .Returns((Events?)null);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
        _loggerMock!.Setup(x => x.LogError(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);

        _loggerMock.Verify(
            x => x.LogError($"Event with ID: {eventId} was not found."),
            Times.Once());
        _eventRepositoryMock.Verify(
            x => x.GetEventById(eventId),
            Times.Once());
        _ticketRepositoryMock!.Verify(
            x => x.GetWeeklyTicketCountAsync(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<TicketType?>()),
            Times.Never());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        // Arrange
        int eventId = 4;
        var query = new GetWeeklyEventTicketCountQuery(eventId, null, null);
        var exception = new Exception("Database connection failed");

        _eventRepositoryMock!
            .Setup(x => x.GetEventById(eventId))
            .Returns(new Events());

        _ticketRepositoryMock!
            .Setup(x => x.GetWeeklyTicketCountAsync(eventId, null, null, null))
            .ThrowsAsync(exception);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
        _loggerMock!.Setup(x => x.LogError(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(ErrorType.Failure, result.FirstError.Type);
        Assert.IsTrue(result.FirstError.Description.Contains("Database connection failed"));

        _loggerMock.Verify(
            x => x.LogError($"Error fetching weekly ticket counts for EventId {eventId}: {exception.Message}"),
            Times.Once());
        _ticketRepositoryMock.Verify(
            x => x.GetWeeklyTicketCountAsync(eventId, null, null, null),
            Times.Once());
        _eventRepositoryMock.Verify(
            x => x.GetEventById(eventId),
            Times.Once());
    }
}