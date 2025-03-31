using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.GetAllEvents;
using Fliq.Domain.Entities.Event.Enums;
using Moq;

[TestClass]
public class GetAllFlaggedEventsQueryHandlerTests
{
    private Mock<IEventRepository>? _eventRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private GetAllFlaggedEventsQueryHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetAllFlaggedEventsQueryHandler(_eventRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(1, 5);
        var query = new GetAllFlaggedEventsQuery(paginationRequest);

        var request = new GetEventsListRequest
        {
            PaginationRequest = paginationRequest,
            Category = null,
            Status = null,
            StartDate = null,
            EndDate = null,
            Location = null
        };

        var createdOn = DateTime.UtcNow;
        var expectedResults = new List<GetEventsResult>
        {
            new GetEventsResult("Flagged Event", "Jane Doe", EventStatus.Upcoming.ToString(), 10, "paid", createdOn)
        };

        _eventRepositoryMock!
            .Setup(x => x.GetAllFlaggedEventsForDashBoardAsync(It.Is<GetEventsListRequest>(
                r => r.PaginationRequest!.PageNumber == paginationRequest.PageNumber &&
                     r.PaginationRequest.PageSize == paginationRequest.PageSize &&
                     r.Category == query.Category &&
                     r.Status == query.Status &&
                     r.StartDate == query.StartDate &&
                     r.EndDate == query.EndDate &&
                     r.Location == query.Location)))
            .ReturnsAsync(expectedResults);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(expectedResults.Count, result.Value.Count);

        var expected = expectedResults[0];
        var actual = result.Value[0];
        Assert.AreEqual(expected.EventTitle, actual.EventTitle);
        Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
        Assert.AreEqual(expected.Status, actual.Status);
        Assert.AreEqual(expected.Attendees, actual.Attendees);
        Assert.AreEqual(expected.Type, actual.Type);
        Assert.IsTrue(Math.Abs((expected.CreatedOn - actual.CreatedOn).TotalSeconds) < 1);

        _loggerMock.Verify(x => x.LogInfo($"Getting flagged events for page 1 with page size 5"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Got 1 flagged events for page 1"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllFlaggedEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }

    [TestMethod]
    public async Task Handle_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(2, 5);
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow.AddDays(1);
        var query = new GetAllFlaggedEventsQuery(paginationRequest, "Concert", EventStatus.Ongoing, startDate, endDate, "London");

        var request = new GetEventsListRequest
        {
            PaginationRequest = paginationRequest,
            Category = "Concert",
            Status = EventStatus.Ongoing,
            StartDate = startDate,
            EndDate = endDate,
            Location = "London"
        };

        var createdOn = DateTime.UtcNow;
        var expectedResults = new List<GetEventsResult>
        {
            new GetEventsResult("Flagged Concert", "John Smith", EventStatus.Ongoing.ToString(), 50, "sponsored", createdOn)
        };

        _eventRepositoryMock!
            .Setup(x => x.GetAllFlaggedEventsForDashBoardAsync(It.Is<GetEventsListRequest>(
                r => r.PaginationRequest!.PageNumber == paginationRequest.PageNumber &&
                     r.PaginationRequest.PageSize == paginationRequest.PageSize &&
                     r.Category == query.Category &&
                     r.Status == query.Status &&
                     r.StartDate == query.StartDate &&
                     r.EndDate == query.EndDate &&
                     r.Location == query.Location)))
            .ReturnsAsync(expectedResults);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(expectedResults.Count, result.Value.Count);

        var expected = expectedResults[0];
        var actual = result.Value[0];
        Assert.AreEqual(expected.EventTitle, actual.EventTitle);
        Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
        Assert.AreEqual(expected.Status, actual.Status);
        Assert.AreEqual(expected.Attendees, actual.Attendees);
        Assert.AreEqual(expected.Type, actual.Type);
        Assert.IsTrue(Math.Abs((expected.CreatedOn - actual.CreatedOn).TotalSeconds) < 1);

        _loggerMock.Verify(x => x.LogInfo($"Getting flagged events for page 2 with page size 5"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Got 1 flagged events for page 2"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllFlaggedEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(1, 5);
        var query = new GetAllFlaggedEventsQuery(paginationRequest);

        var request = new GetEventsListRequest
        {
            PaginationRequest = paginationRequest,
            Category = null,
            Status = null,
            StartDate = null,
            EndDate = null,
            Location = null
        };

        var exception = new Exception("Database connection failed");

        _eventRepositoryMock!
            .Setup(x => x.GetAllFlaggedEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
            .ThrowsAsync(exception);

        _loggerMock!.Setup(x => x.LogInfo($"Getting flagged events for page 1 with page size 5"));
        _loggerMock.Setup(x => x.LogError($"Error fetching flagged events: {exception.Message}"));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("GetFlaggedEventsFailed", result.FirstError.Code);
        Assert.IsTrue(result.FirstError.Description.Contains("Database connection failed"));

        _loggerMock.Verify(x => x.LogInfo($"Getting flagged events for page 1 with page size 5"), Times.Once());
        _loggerMock.Verify(x => x.LogError($"Error fetching flagged events: {exception.Message}"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllFlaggedEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }
}