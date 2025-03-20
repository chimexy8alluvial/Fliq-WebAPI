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
    public class GetAllCancelledEventsQueryHandlerTests
    {
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetAllCancelledEventsQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllCancelledEventsQueryHandler(
                _eventRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ReturnsCancelledEventsList_WhenQueryIsValid()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllCancelledEventsQuery(pagination);
            var events = new List<Events>
            {
                new Events
                {
                    EventTitle = "Cancelled Event",
                    UserId = 1,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2),
                    EventCategory = EventCategory.Free,
                    DateCreated = DateTime.Now,
                    Tickets = new List<Ticket> { new Ticket() },
                    IsCancelled = true
                }
            };
            var user = new User { Id = 1, DisplayName = "Test User", FirstName = "Test", LastName = "User"};
            
            _eventRepositoryMock?
                .Setup(r => r.GetAllCancelledEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
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
            Assert.AreEqual("Cancelled Event", result.Value[0].EventTitle);
            Assert.AreEqual("Test User", result.Value[0].CreatedBy);
            Assert.AreEqual("Upcoming", result.Value[0].Status);
            Assert.AreEqual(1, result.Value[0].Attendees);
            Assert.AreEqual("Free", result.Value[0].EventCategory);
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_WhenNoCancelledEventsFound()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllCancelledEventsQuery(pagination);
            var events = new List<Events>();

            _eventRepositoryMock?
                .Setup(r => r.GetAllCancelledEventsForDashBoardAsync(It.IsAny<GetEventsListRequest>()))
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
            var query = new GetAllCancelledEventsQuery(pagination, null, startDate, endDate);
            var events = new List<Events>
            {
                new Events
                {
                    EventTitle = "Cancelled Ongoing Event",
                    UserId = 1,
                    StartDate = DateTime.Now.AddHours(-1),
                    EndDate = DateTime.Now.AddHours(2),
                    EventCategory = EventCategory.Paid,
                    DateCreated = DateTime.Now,
                    Tickets = new List<Ticket> { new Ticket() },
                    IsCancelled = true
                }
            };
            var user = new User { Id = 1, DisplayName = "Test User" };

            _eventRepositoryMock?
                .Setup(r => r.GetAllCancelledEventsForDashBoardAsync(It.Is<GetEventsListRequest>(req =>
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
            Assert.AreEqual("Cancelled Ongoing Event", result.Value[0].EventTitle);
            Assert.AreEqual("Ongoing", result.Value[0].Status);
            Assert.AreEqual(1, result.Value[0].Attendees);
            Assert.AreEqual("Paid", result.Value[0].EventCategory);
        }

        [TestMethod]
        public async Task Handle_FiltersByCategory_WhenProvided()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var category = "Free";
            var query = new GetAllCancelledEventsQuery(pagination, category);
            var events = new List<Events>
            {
                new Events
                {
                    EventTitle = "Cancelled Free Event",
                    UserId = 1,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2),
                    EventCategory = EventCategory.Free,
                    DateCreated = DateTime.Now,
                    Tickets = new List<Ticket>(),
                    IsCancelled = true
                }
            };
            var user = new User { Id = 1, DisplayName = "Test User" };

            _eventRepositoryMock?
                .Setup(r => r.GetAllCancelledEventsForDashBoardAsync(It.Is<GetEventsListRequest>(req =>
                    req.Category == category)))
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
            Assert.AreEqual("Cancelled Free Event", result.Value[0].EventTitle);
            Assert.AreEqual("Free", result.Value[0].EventCategory);
            Assert.AreEqual(0, result.Value[0].Attendees);
            Assert.AreEqual("Upcoming", result.Value[0].Status);
        }
    }

   
}