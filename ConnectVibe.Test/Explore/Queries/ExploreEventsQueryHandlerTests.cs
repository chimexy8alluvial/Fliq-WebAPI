using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Queries;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;
using Moq;

namespace Fliq.Application.Tests.Explore.Queries
{
    [TestClass]
    public class ExploreEventsQueryHandlerTests
    {
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private ExploreEventsQueryHandler? _handler;
        private CancellationToken _cancellationToken;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new ExploreEventsQueryHandler(
                _userRepositoryMock.Object,
                _eventRepositoryMock.Object,
                _loggerMock.Object);
            _cancellationToken = CancellationToken.None;
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithAllFilters_ReturnsPaginatedEvents()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: 10.0,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatorId: 2,
                Status: EventStatus.Upcoming,
                PageNumber: 1,
                PageSize: 5);

            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } },
                    Gender = new Gender { GenderType = "Male" },
                    Ethnicity = new Ethnicity { EthnicityType = "Asian" },
                    Passions = new List<string> { "music", "comedy" }
                }
            };

            var events = new List<Events>
            {
                new Events { Id = 1, EventTitle = "Test Event", Status = EventStatus.Upcoming }
            };
            var totalCount = 10;

            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock!.Setup(repo => repo.GetEventsAsync(
                It.Is<LocationDetail>(ld => ld.Location.Lat == 40.7128 && ld.Location.Lng == -74.0060),
                It.Is<double?>(d => d == 10.0),
                It.Is<UserProfile>(up => up.Gender.GenderType == "Male" && up.Ethnicity!.EthnicityType == "Asian"),
                It.Is<EventCategory?>(c => c == EventCategory.Free),
                It.Is<EventType?>(t => t == EventType.Live),
                It.Is<int?>(c => c == 2),
                It.Is<EventStatus?>(s => s == EventStatus.Upcoming),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.AreEqual(events, response.Data);
            Assert.AreEqual(totalCount, response.TotalCount);
            Assert.AreEqual(query.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PageSize, response.PageSize);
            _loggerMock!.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetching events"))), Times.Once());
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetched 10 events"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithoutStatus_ReturnsPaginatedEvents()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: 10.0,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatorId: 2,
                Status: null, // No status filter
                PageNumber: 1,
                PageSize: 5);

            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };

            var events = new List<Events>
            {
                new Events { Id = 1, EventTitle = "Test Event" }
            };
            var totalCount = 5;

            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock!.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.IsAny<int?>(),
                It.Is<EventStatus?>(s => s == null),
                It.IsAny<PaginationRequest>()))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.AreEqual(events, response.Data);
            Assert.AreEqual(totalCount, response.TotalCount);
            Assert.AreEqual(query.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PageSize, response.PageSize);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns((User)null!);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.Errors[0]);
            _loggerMock!.Verify(l => l.LogWarn(It.Is<string>(s => s.Contains("User not found"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_UserProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            var user = new User { Id = 1, UserProfile = null };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.ProfileNotFound, result.Errors[0]);
            _loggerMock!.Verify(l => l.LogWarn(It.Is<string>(s => s.Contains("UserProfile not found"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_LocationNotConfigured_ReturnsLocationNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile { Location = null }
            };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Profile.LocationNotFound", result.Errors[0].Code);
            Assert.AreEqual("User location is not configured.", result.Errors[0].Description);
            _loggerMock!.Verify(l => l.LogWarn(It.Is<string>(s => s.Contains("User location not found"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_LocationDetailNotConfigured_ReturnsLocationNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile { Location = new Location { LocationDetail = null! } }
            };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Profile.LocationNotFound", result.Errors[0].Code);
            Assert.AreEqual("User location is not configured.", result.Errors[0].Description);
            _loggerMock!.Verify(l => l.LogWarn(It.Is<string>(s => s.Contains("User location not found"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_RepositoryThrowsException_ReturnsFailureError()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock!.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.IsAny<int?>(),
                It.IsAny<EventStatus?>(),
                It.IsAny<PaginationRequest>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Failure", result.Errors[0].Type.ToString());
            Assert.IsTrue(result.Errors[0].Description.Contains("Failed to fetch events: Database error"));
            _loggerMock!.Verify(l => l.LogError(It.Is<string>(s => s.Contains("Failed to fetch events"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_NullEventsList_ReturnsEmptyPaginatedResponse()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock!.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.IsAny<int?>(),
                It.IsAny<EventStatus?>(),
                It.IsAny<PaginationRequest>()))!
                .ReturnsAsync((null, 0));

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.Count());
            Assert.AreEqual(0, response.TotalCount);
            Assert.AreEqual(query.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PageSize, response.PageSize);
        }

        [TestMethod]
        public async Task Handle_EmptyEventsList_ReturnsEmptyPaginatedResponse()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };
            var events = new List<Events>();
            var totalCount = 0;

            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock!.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.IsAny<int?>(),
                It.IsAny<EventStatus?>(),
                It.IsAny<PaginationRequest>()))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.Count());
            Assert.AreEqual(0, response.TotalCount);
            Assert.AreEqual(query.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PageSize, response.PageSize);
        }
    }
}