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
        var paginationRequest = new PaginationRequest(1, 10);
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

        var expectedResults = new List<GetEventsResult>
        {
            new GetEventsResult("Test Flagged Event", "John Doe", "Upcoming", 5, "free", DateTime.UtcNow)
        };

        _eventRepositoryMock!
            .Setup(x => x.GetAllFlaggedEventsForDashBoardAsync(It.Is<GetEventsListRequest>(
                r => r.PaginationRequest!.PageNumber == 1 &&
                     r.PaginationRequest.PageSize == 10 &&
                     r.Category == null &&
                     r.Status == null &&
                     r.StartDate == null &&
                     r.EndDate == null &&
                     r.Location == null)))
            .ReturnsAsync(expectedResults);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(expectedResults, result.Value);
        Assert.AreEqual(1, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Getting flagged events for page 1 with page size 10"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Got 1 flagged events for page 1"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllFlaggedEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }

    [TestMethod]
    public async Task Handle_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(2, 5);
        var query = new GetAllFlaggedEventsQuery(
            paginationRequest,
            "Music",
            EventStatus.Ongoing,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            "New York");

        var request = new GetEventsListRequest
        {
            PaginationRequest = paginationRequest,
            Category = "Music",
            Status = EventStatus.Ongoing,
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            Location = "New York"
        };

        var expectedResults = new List<GetEventsResult>
        {
            new GetEventsResult("Flagged Music Fest", "Jane Doe", "Ongoing", 100, "sponsored", DateTime.UtcNow)
        };

        _eventRepositoryMock!
            .Setup(x => x.GetAllFlaggedEventsForDashBoardAsync(It.Is<GetEventsListRequest>(
                r => r.PaginationRequest!.PageNumber == 2 &&
                     r.PaginationRequest.PageSize == 5 &&
                     r.Category == "Music" &&
                     r.Status == EventStatus.Ongoing &&
                     r.StartDate == query.StartDate &&
                     r.EndDate == query.EndDate &&
                     r.Location == "New York")))
            .ReturnsAsync(expectedResults);

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(expectedResults, result.Value);
        Assert.AreEqual(1, result.Value.Count);

        _loggerMock.Verify(x => x.LogInfo($"Getting flagged events for page 2 with page size 5"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Got 1 flagged events for page 2"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllFlaggedEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(1, 10);
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

        _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
        _loggerMock.Setup(x => x.LogError(It.IsAny<string>()));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("GetEventsFailed", result.Errors[0].Code);
        Assert.IsTrue(result.Errors[0].Description.Contains("Database connection failed"));

        _loggerMock.Verify(x => x.LogInfo($"Getting flagged events for page 1 with page size 10"), Times.Once());
        _loggerMock.Verify(x => x.LogError($"Error fetching events: {exception.Message}"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllFlaggedEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }
}