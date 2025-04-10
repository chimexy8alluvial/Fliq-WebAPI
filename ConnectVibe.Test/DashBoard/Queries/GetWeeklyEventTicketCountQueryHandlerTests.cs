using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.DailyTicketCount;
using Fliq.Domain.Entities.Event;
using Moq;

[TestClass]
public class GetWeeklyEventTicketCountQueryHandlerTests
{
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private GetWeeklyEventTicketCountQueryHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetWeeklyEventTicketCountQueryHandler(_ticketRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidEventIdWithRange_ReturnsWeeklyCountResult()
    {
        int eventId = 1;
        DateTime? startDate = new DateTime(2025, 4, 1);
        DateTime? endDate = new DateTime(2025, 4, 7);
        var query = new GetWeeklyEventTicketCountQuery(eventId, startDate, endDate);

        var expectedCounts = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Sunday, 2 }, { DayOfWeek.Monday, 0 }, { DayOfWeek.Tuesday, 1 },
            { DayOfWeek.Wednesday, 3 }, { DayOfWeek.Thursday, 0 }, { DayOfWeek.Friday, 5 },
            { DayOfWeek.Saturday, 1 }
        };

        _ticketRepositoryMock!
            .Setup(x => x.GetWeeklyTicketCountAsync(eventId, startDate, endDate, null))
            .ReturnsAsync(expectedCounts);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(WeeklyCountResult));
        CollectionAssert.AreEqual(expectedCounts, result.Value.DailyCounts);
    }

    [TestMethod]
    public async Task Handle_NullDateRange_DefaultsToLastWeek()
    {
        int eventId = 2;
        DateTime? startDate = null;
        DateTime? endDate = null;
        var query = new GetWeeklyEventTicketCountQuery(eventId, startDate, endDate, TicketType.Vip);

        var today = DateTime.UtcNow.Date;
        var expectedStartDate = today.AddDays(-6);
        var expectedCounts = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Sunday, 1 }, { DayOfWeek.Monday, 0 }, { DayOfWeek.Tuesday, 0 },
            { DayOfWeek.Wednesday, 2 }, { DayOfWeek.Thursday, 0 }, { DayOfWeek.Friday, 3 },
            { DayOfWeek.Saturday, 0 }
        };

        _ticketRepositoryMock!
            .Setup(x => x.GetWeeklyTicketCountAsync(eventId, expectedStartDate, today, TicketType.Vip))
            .ReturnsAsync(expectedCounts);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(WeeklyCountResult));
        CollectionAssert.AreEqual(expectedCounts, result.Value.DailyCounts);

        _loggerMock.Verify(x => x.LogInfo($"Fetching weekly ticket counts for EventId: {eventId}, " +
                                          $"Date Range: Default Start to Default End, TicketType: Vip"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NullStartDate_DefaultsToWeekBeforeEndDate()
    {
        int eventId = 3;
        DateTime? startDate = null;
        DateTime? endDate = new DateTime(2025, 4, 7);
        var query = new GetWeeklyEventTicketCountQuery(eventId, startDate, endDate);

        var expectedStartDate = endDate.Value.AddDays(-6);
        var expectedCounts = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Sunday, 0 }, { DayOfWeek.Monday, 0 }, { DayOfWeek.Tuesday, 0 },
            { DayOfWeek.Wednesday, 0 }, { DayOfWeek.Thursday, 0 }, { DayOfWeek.Friday, 0 },
            { DayOfWeek.Saturday, 0 }
        };

        _ticketRepositoryMock!
            .Setup(x => x.GetWeeklyTicketCountAsync(eventId, expectedStartDate, endDate, null))
            .ReturnsAsync(expectedCounts);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(WeeklyCountResult));
        CollectionAssert.AreEqual(expectedCounts, result.Value.DailyCounts);
    }

    [TestMethod]
    public async Task Handle_NullEndDate_DefaultsToWeekFromStartDate()
    {
        int eventId = 4;
        DateTime? startDate = new DateTime(2025, 4, 1);
        DateTime? endDate = null;
        var query = new GetWeeklyEventTicketCountQuery(eventId, startDate, endDate);

        var today = DateTime.UtcNow.Date;
        var effectiveEndDate = startDate.Value > today.AddDays(-6) ? today : startDate.Value.AddDays(6);
        var effectiveStartDate = effectiveEndDate.AddDays(-6);
        var expectedCounts = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Sunday, 1 }, { DayOfWeek.Monday, 1 }, { DayOfWeek.Tuesday, 1 },
            { DayOfWeek.Wednesday, 1 }, { DayOfWeek.Thursday, 1 }, { DayOfWeek.Friday, 1 },
            { DayOfWeek.Saturday, 1 }
        };

        _ticketRepositoryMock!
            .Setup(x => x.GetWeeklyTicketCountAsync(eventId, startDate, effectiveEndDate, null))
            .ReturnsAsync(expectedCounts);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        var result = await _handler!.Handle(query, CancellationToken.None);

        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType(result.Value, typeof(WeeklyCountResult));
        CollectionAssert.AreEqual(expectedCounts, result.Value.DailyCounts);
    }
}