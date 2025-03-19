using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Moq;

namespace Fliq.Application.DashBoard.Queries.GetAllEvents.Tests
{
    [TestClass]
    public class GetAllEventsQueryHandlerTests
    {
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetAllEventsQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllEventsQueryHandler(
                _eventRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ReturnsEventsList_WhenQueryIsValid()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllEventsQuery(pagination);
            var events = new List<Events>
            {
                new Events
                {
                    EventTitle = "Test Event",
                    UserId = 1,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2),
                    EventCategory = EventCategory.Free,
                    DateCreated = DateTime.Now,
                    Tickets = new List<Ticket> { new Ticket() }
                }
            };
            var user = new User { Id = 1, DisplayName = "Test User" };

            _eventRepositoryMock?
                .Setup(r => r.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
                .ReturnsAsync(events);
            _userRepositoryMock?
                .Setup(r => r.GetUserById(1))
                .Returns(user);
            _loggerMock?.Setup(l => l.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual("Test Event", result.Value[0].EventTitle);
            Assert.AreEqual("Test User", result.Value[0].CreatedBy);
            Assert.AreEqual("Upcoming", result.Value[0].Status);
            Assert.AreEqual(1, result.Value[0].Attendees);
            Assert.AreEqual("Free", result.Value[0].EventCategory);
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_WhenNoEventsFound()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllEventsQuery(pagination);
            var events = new List<Events>();

            _eventRepositoryMock?
                .Setup(r => r.GetAllEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
                .ReturnsAsync(events);
            _loggerMock?.Setup(l => l.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Value.Count);
        }

        [TestMethod]
        public async Task Handle_FiltersByDateRange_WhenProvided()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var startDate = DateTime.Now.AddDays(-1);
            var endDate = DateTime.Now.AddDays(1);
            var query = new GetAllEventsQuery(pagination, null, startDate, endDate);
            var events = new List<Events>
            {
                new Events
                {
                    EventTitle = "Test Event",
                    UserId = 1,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddHours(2),
                    EventCategory = EventCategory.Paid,
                    DateCreated = DateTime.Now,
                    Tickets = new List<Ticket> { new Ticket() }
                }
            };
            var user = new User { Id = 1, DisplayName = "Test User" };

            _eventRepositoryMock?
                .Setup(r => r.GetAllEventsForDashBoardAsync(It.Is<GetEventsListRequest>(req =>
                    req.StartDate == startDate && req.EndDate == endDate)))
                .ReturnsAsync(events);
            _userRepositoryMock?
                .Setup(r => r.GetUserById(1))
                .Returns(user);
            _loggerMock?.Setup(l => l.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual("Ongoing", result.Value[0].Status);
        }
    }

    
}