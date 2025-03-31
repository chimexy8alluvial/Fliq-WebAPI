using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.GetAllEvents;
using Fliq.Domain.Entities.Event.Enums;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[TestClass]
public class GetAllEventsQueryHandlerTests
{
    private Mock<IEventRepository> _eventRepositoryMock;
    private Mock<ILoggerManager> _loggerMock;
    private GetAllEventsQueryHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new GetAllEventsQueryHandler(_eventRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(1, 10);
        var query = new GetAllEventsQuery(paginationRequest);

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
            new GetEventsResult("Test Event", "John Doe", EventStatus.Upcoming.ToString(), 5, "free", createdOn)
        };

        _eventRepositoryMock
            .Setup(x => x.GetAllEventsForDashBoardAsync(It.Is<GetEventsListRequest>(
                r => r.PaginationRequest.PageNumber == 1 &&
                     r.PaginationRequest.PageSize == 10 &&
                     r.Category == null &&
                     r.Status == null &&
                     r.StartDate == null &&
                     r.EndDate == null &&
                     r.Location == null)))
            .ReturnsAsync(expectedResults);

        _loggerMock.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

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

        _loggerMock.Verify(x => x.LogInfo($"Getting events for page 1 with page size 10"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Got 1 events for page 1"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }

    [TestMethod]
    public async Task Handle_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(2, 5);
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow.AddDays(1);
        var query = new GetAllEventsQuery(paginationRequest, "Music", EventStatus.Ongoing, startDate, endDate, "New York");

        var request = new GetEventsListRequest
        {
            PaginationRequest = paginationRequest,
            Category = "Music",
            Status = EventStatus.Ongoing,
            StartDate = startDate,
            EndDate = endDate,
            Location = "New York"
        };

        var createdOn = DateTime.UtcNow;
        var expectedResults = new List<GetEventsResult>
        {
            new GetEventsResult("Music Fest", "Jane Doe", EventStatus.Ongoing.ToString(), 100, "sponsored", createdOn)
        };

        _eventRepositoryMock
            .Setup(x => x.GetAllEventsForDashBoardAsync(It.Is<GetEventsListRequest>(
                r => r.PaginationRequest.PageNumber == 2 &&
                     r.PaginationRequest.PageSize == 5 &&
                     r.Category == "Music" &&
                     r.Status == EventStatus.Ongoing &&
                     r.StartDate == startDate &&
                     r.EndDate == endDate &&
                     r.Location == "New York")))
            .ReturnsAsync(expectedResults);

        _loggerMock.Setup(x => x.LogInfo(It.IsAny<string>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

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

        _loggerMock.Verify(x => x.LogInfo($"Getting events for page 2 with page size 5"), Times.Once());
        _loggerMock.Verify(x => x.LogInfo($"Got 1 events for page 2"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(1, 10);
        var query = new GetAllEventsQuery(paginationRequest);

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

        _eventRepositoryMock
            .Setup(x => x.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
            .ThrowsAsync(exception);

        _loggerMock.Setup(x => x.LogInfo(It.IsAny<string>()));
        _loggerMock.Setup(x => x.LogError(It.IsAny<string>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("GetEventsFailed", result.FirstError.Code);
        Assert.IsTrue(result.FirstError.Description.Contains("Database connection failed"));

        _loggerMock.Verify(x => x.LogInfo($"Getting events for page 1 with page size 10"), Times.Once());
        _loggerMock.Verify(x => x.LogError($"Error fetching events: {exception.Message}"), Times.Once());
        _eventRepositoryMock.Verify(x => x.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()), Times.Once());
    }
}