using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.GetAllEvents;
using Fliq.Domain.Entities.Event;
using Moq;

namespace Fliq.Application.Tests.DashBoard.Queries
{
    [TestClass]
    public class GetAllEventsQueryHandlerTests
    {
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetAllEventsQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllEventsQueryHandler(_eventRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsListOfGetEventsResult()
        {
            // Arrange
            var query = new GetAllEventsQuery(
                PaginationRequest: new PaginationRequest(1, 10),
                Category: "Music",
                Status: null,
                StartDate: null,
                EndDate: null,
                Location: "New York"
            );

            var eventWithUsernames = new List<EventWithUsername>
            {
                new EventWithUsername
                {
                    Event = new Events
                    {
                        EventTitle = "Concert Night",
                        UserId = 1,
                        StartDate = DateTime.Now.AddDays(1),
                        EndDate = DateTime.Now.AddDays(2),
                        SponsoredEvent = true,
                        DateCreated = DateTime.Now.AddDays(-5),
                        Tickets = new List<Ticket> { new Ticket { Id = 1 }, new Ticket { Id = 2 } }
                    },
                    Username = "John Doe",
                    CalculatedStatus = "Upcoming"
                },
                new EventWithUsername
                {
                    Event = new Events
                    {
                        EventTitle = "Art Exhibition",
                        UserId = 2,
                        StartDate = DateTime.Now.AddDays(-1),
                        EndDate = DateTime.Now.AddDays(1),
                        SponsoredEvent = false,
                        DateCreated = DateTime.Now.AddDays(-3),
                        Tickets = new List<Ticket> { new Ticket { Id = 3 } }
                    },
                    Username = "Jane Smith",
                    CalculatedStatus = "Ongoing"
                }
            };

            _eventRepositoryMock!
                .Setup(repo => repo.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
                .ReturnsAsync(eventWithUsernames);

            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Value, typeof(List<GetEventsResult>)); // Check return type
            Assert.AreEqual(2, result.Value.Count); // Check number of results

            var firstResult = result.Value[0];
            Assert.AreEqual("Concert Night", firstResult.EventTitle);
            Assert.AreEqual("John Doe", firstResult.CreatedBy);
            Assert.AreEqual("Upcoming", firstResult.Status);
            Assert.AreEqual(2, firstResult.Attendees);
            Assert.AreEqual("sponsored", firstResult.Type);

            var secondResult = result.Value[1];
            Assert.AreEqual("Art Exhibition", secondResult.EventTitle);
            Assert.AreEqual("Jane Smith", secondResult.CreatedBy);
            Assert.AreEqual("Ongoing", secondResult.Status);
            Assert.AreEqual(1, secondResult.Attendees);
            Assert.AreEqual("free", secondResult.Type);

            // Verify logging
            _loggerMock.Verify(logger => logger.LogInfo($"Getting events for page {query.PaginationRequest.PageNumber} with page size {query.PaginationRequest.PageSize}"), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo($"Got {eventWithUsernames.Count} events for page {query.PaginationRequest.PageNumber}"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EmptyResult_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetAllEventsQuery(
                PaginationRequest: new PaginationRequest(1, 10)
            );

            _eventRepositoryMock!
                .Setup(repo => repo.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
                .ReturnsAsync(new List<EventWithUsername>());

            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Value, typeof(List<GetEventsResult>));
            Assert.AreEqual(0, result.Value.Count);

            _loggerMock.Verify(logger => logger.LogInfo("Got 0 events for page 1"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_NullTickets_ReturnsZeroAttendees()
        {
            // Arrange
            var query = new GetAllEventsQuery(
                PaginationRequest: new PaginationRequest(1, 10)
            );

            var eventWithUsernames = new List<EventWithUsername>
            {
                new EventWithUsername
                {
                    Event = new Events
                    {
                        EventTitle = "Workshop",
                        UserId = 1,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(1),
                        SponsoredEvent = false,
                        DateCreated = DateTime.Now,
                        Tickets = null // No tickets
                    },
                    Username = "Test User",
                    CalculatedStatus = "Ongoing"
                }
            };

            _eventRepositoryMock!
                .Setup(repo => repo.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
                .ReturnsAsync(eventWithUsernames);

            _loggerMock!.Setup(logger => logger.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert        
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual(0, result.Value[0].Attendees); // Should handle null Tickets
            Assert.AreEqual("Ongoing", result.Value[0].Status);
        }
    }
}