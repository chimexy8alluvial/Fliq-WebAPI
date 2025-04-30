using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.Tests.DashBoard.Queries.GetEventsTicket;
using Fliq.Domain.Entities.Event.Enums;
using Moq;

namespace Fliq.Application.Tests.DashBoard.Queries.GetAllEvents
{
    [TestClass]
    public class GetAllEventsTicketsQueryHandlerTests
    {
        private Mock<ITicketRepository>? _ticketRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetAllEventsTicketsQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllEventsTicketsQueryHandler(_ticketRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ValidRequestNoFilter_ReturnsSuccessResult()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(1, 5);
            var query = new GetAllEventsTicketsQuery(paginationRequest);

            var createdOn = DateTime.UtcNow;
            var expectedResults = new List<GetEventsTicketsResult>
            {
                new GetEventsTicketsResult("Test Event", "John Doe", "Upcoming", "free", 5, createdOn)
            };

            _ticketRepositoryMock!
                .Setup(x => x.GetAllEventsTicketsForDashBoardAsync(It.Is<GetEventsTicketsListRequest>(
                    r => r.PaginationRequest.PageNumber == paginationRequest.PageNumber &&
                         r.PaginationRequest.PageSize == paginationRequest.PageSize &&
                         r.Category == query.Category &&
                         r.StatusFilter == query.StatusFilter &&
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
            Assert.AreEqual(expected.EventStatus, actual.EventStatus);
            Assert.AreEqual(expected.NatureOfEvent, actual.NatureOfEvent);
            Assert.AreEqual(expected.NumOfSoldTickets, actual.NumOfSoldTickets);
            Assert.IsTrue(Math.Abs((expected.CreatedOn - actual.CreatedOn).TotalSeconds) < 1);

            _loggerMock.Verify(x => x.LogInfo($"Getting events with tickets for page 1 with page size 5"), Times.Once());
            _loggerMock.Verify(x => x.LogInfo($"Got 1 events with tickets for page 1"), Times.Once());
            _ticketRepositoryMock.Verify(x => x.GetAllEventsTicketsForDashBoardAsync(It.IsAny<GetEventsTicketsListRequest>()), Times.Once());
        }

        [TestMethod]
        public async Task Handle_WithSoldOutFilter_ReturnsFilteredResults()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(2, 5);
            var startDate = DateTime.UtcNow.AddDays(-1);
            var endDate = DateTime.UtcNow.AddDays(1);
            var query = new GetAllEventsTicketsQuery(
                paginationRequest,
                EventCategory.Paid,
                "SoldOut",
                startDate,
                endDate,
                "New York" // Test with a city name
            );

            var createdOn = DateTime.UtcNow;
            var expectedResults = new List<GetEventsTicketsResult>
    {
        new GetEventsTicketsResult("Music Fest", "Jane Smith", "Ongoing", "sponsored", 100, createdOn)
    };

            _ticketRepositoryMock!
                .Setup(x => x.GetAllEventsTicketsForDashBoardAsync(It.Is<GetEventsTicketsListRequest>(
                    r => r.PaginationRequest.PageNumber == paginationRequest.PageNumber &&
                         r.PaginationRequest.PageSize == paginationRequest.PageSize &&
                         r.Category == query.Category &&
                         r.StatusFilter == query.StatusFilter &&
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
            Assert.AreEqual(expected.EventStatus, actual.EventStatus);
            Assert.AreEqual(expected.NatureOfEvent, actual.NatureOfEvent);
            Assert.AreEqual(expected.NumOfSoldTickets, actual.NumOfSoldTickets);
            Assert.IsTrue(Math.Abs((expected.CreatedOn - actual.CreatedOn).TotalSeconds) < 1);

            _loggerMock.Verify(x => x.LogInfo($"Getting events with tickets for page 2 with page size 5"), Times.Once());
            _loggerMock.Verify(x => x.LogInfo($"Got 1 events with tickets for page 2"), Times.Once());
            _ticketRepositoryMock.Verify(x => x.GetAllEventsTicketsForDashBoardAsync(It.IsAny<GetEventsTicketsListRequest>()), Times.Once());
        }

        [TestMethod]
        public async Task Handle_WithCancelledFilter_ReturnsFilteredResults()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(1, 5);
            var query = new GetAllEventsTicketsQuery(
                paginationRequest,
                EventCategory.Free, // Updated to use EventCategory enum
                "Cancelled"
            );

            var createdOn = DateTime.UtcNow;
            var expectedResults = new List<GetEventsTicketsResult>
            {
                new GetEventsTicketsResult("Cancelled Event", "John Doe", "Cancelled", "free", 0, createdOn)
            };

            _ticketRepositoryMock!
                .Setup(x => x.GetAllEventsTicketsForDashBoardAsync(It.Is<GetEventsTicketsListRequest>(
                    r => r.PaginationRequest.PageNumber == paginationRequest.PageNumber &&
                         r.PaginationRequest.PageSize == paginationRequest.PageSize &&
                         r.Category == query.Category &&
                         r.StatusFilter == query.StatusFilter &&
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
            Assert.AreEqual(expected.EventStatus, actual.EventStatus);
            Assert.AreEqual(expected.NatureOfEvent, actual.NatureOfEvent);
            Assert.AreEqual(expected.NumOfSoldTickets, actual.NumOfSoldTickets);
            Assert.IsTrue(Math.Abs((expected.CreatedOn - actual.CreatedOn).TotalSeconds) < 1);

            _loggerMock.Verify(x => x.LogInfo($"Getting events with tickets for page 1 with page size 5"), Times.Once());
            _loggerMock.Verify(x => x.LogInfo($"Got 1 events with tickets for page 1"), Times.Once());
            _ticketRepositoryMock.Verify(x => x.GetAllEventsTicketsForDashBoardAsync(It.IsAny<GetEventsTicketsListRequest>()), Times.Once());
        }

        [TestMethod]
        public async Task Handle_RepositoryThrowsException_ReturnsError()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(1, 5);
            var query = new GetAllEventsTicketsQuery(paginationRequest);

            var exception = new Exception("Database connection failed");

            _ticketRepositoryMock!
                .Setup(x => x.GetAllEventsTicketsForDashBoardAsync(It.IsAny<GetEventsTicketsListRequest>()))
                .ThrowsAsync(exception);

            _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
            _loggerMock!.Setup(x => x.LogError(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("GetEventsTicketsFailed", result.FirstError.Code);
            Assert.IsTrue(result.FirstError.Description.Contains("Database connection failed"));

            _loggerMock.Verify(x => x.LogInfo($"Getting events with tickets for page 1 with page size 5"), Times.Once());
            _loggerMock.Verify(x => x.LogError($"Error fetching events with tickets: {exception.Message}"), Times.Once());
            _ticketRepositoryMock.Verify(x => x.GetAllEventsTicketsForDashBoardAsync(It.IsAny<GetEventsTicketsListRequest>()), Times.Once());
        }
    }
}