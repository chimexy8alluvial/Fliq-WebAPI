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
using Newtonsoft.Json;

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
                MaxDistanceKm: 10,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatorId: 2,
                Status: EventStatus.Upcoming,
                PageNumber: 1,
                PageSize: 5);

            var user = CreateValidUser();
            var events = new List<Events> { CreateValidEvent() };
            var totalCount = 10;

            _userRepositoryMock?.Setup(repo => repo.GetUserById(1))
                .Returns(user)
                .Verifiable();

            _eventRepositoryMock?.Setup(repo => repo.GetEventsAsync(
                It.Is<LocationDetail>(ld => ld != null),
                It.Is<double?>(d => d == 10),
                It.Is<UserProfile>(up => up.UserId == 1),
                It.Is<EventCategory?>(c => c == EventCategory.Free),
                It.Is<EventType?>(et => et == EventType.Live),
                It.Is<int?>(cid => cid == 2),
                It.Is<EventStatus?>(s => s == EventStatus.Upcoming),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events.AsEnumerable(), totalCount))
                .Verifiable();

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError, "Expected no error, but an error was returned: {0}", result.Errors?.FirstOrDefault().Description);
            Assert.IsNotNull(result.Value, "result.Value is null");
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            Assert.IsNotNull(result.Value.Events, "result.Value.Events is null");
            Assert.IsNotNull(result.Value.Events.Data, "result.Value.Events.Data is null");
            Assert.AreEqual(events.Count, result.Value.Events.Data.Count());
            Assert.AreEqual(totalCount, result.Value.Events.TotalCount);
            Assert.AreEqual(query.PageNumber, result.Value.Events.PageNumber);
            Assert.AreEqual(query.PageSize, result.Value.Events.PageSize);
            _userRepositoryMock?.Verify();
            _eventRepositoryMock?.Verify();
            _loggerMock?.Verify(x => x.LogInfo($"Fetched {totalCount} events for user {user.Id}. Page {query.PageNumber} has {events.Count} items."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithoutStatus_ReturnsPaginatedEvents()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: null,
                Category: null,
                EventType: null,
                CreatorId: null,
                Status: null,
                PageNumber: 1,
                PageSize: 5);

            var user = CreateValidUser();
            var events = new List<Events> { CreateValidEvent() };
            var totalCount = 10;

            _userRepositoryMock?.Setup(repo => repo.GetUserById(1))
                .Returns(user)
                .Verifiable();

            _eventRepositoryMock?.Setup(repo => repo.GetEventsAsync(
                It.Is<LocationDetail>(ld => ld != null && ld.Location != null && ld.Location.Lat == 40.7128 && ld.Location.Lng == -74.0060),
                It.Is<double?>(d => d == null),
                It.Is<UserProfile>(up => up.UserId == 1),
                It.Is<EventCategory?>(c => c == null),
                It.Is<EventType?>(et => et == null),
                It.Is<int?>(cid => cid == null),
                It.Is<EventStatus?>(s => s == null),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events.AsEnumerable(), totalCount))
                .Callback(() => Console.WriteLine("Mock GetEventsAsync invoked"))
                .Verifiable();

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Console.WriteLine($"Result.IsError: {result.IsError}");
            Console.WriteLine($"Result.Value: {result.Value != null}");
            Console.WriteLine($"Result.Value.Events: {result.Value?.Events != null}");
            Console.WriteLine($"Result.Value.Events.Data: {result.Value?.Events?.Data != null}");
            Console.WriteLine($"Result.Value.Events.Data.Count: {result.Value?.Events?.Data?.Count() ?? -1}");

            Assert.IsFalse(result.IsError, "Expected no error, but an error was returned: {0}", result.Errors?.FirstOrDefault().Description);
            Assert.IsNotNull(result.Value, "result.Value is null");
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            Assert.IsNotNull(result.Value.Events, "result.Value.Events is null");
            Assert.IsNotNull(result.Value.Events.Data, "result.Value.Events.Data is null");
            Assert.AreEqual(events.Count, result.Value.Events.Data.Count());
            Assert.AreEqual(totalCount, result.Value.Events.TotalCount);
            Assert.AreEqual(query.PageNumber, result.Value.Events.PageNumber);
            Assert.AreEqual(query.PageSize, result.Value.Events.PageSize);
            _userRepositoryMock?.Verify();
            _eventRepositoryMock?.Verify();
            _loggerMock?.Verify(x => x.LogInfo($"Fetched {totalCount} events for user {user.Id}. Page {query.PageNumber} has {events.Count} items."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EmptyEventsList_ReturnsEmptyPaginatedResponse()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                PageNumber: 1,
                PageSize: 5);

            var user = CreateValidUser();
            var events = new List<Events>();
            var totalCount = 0;

            _userRepositoryMock?.Setup(repo => repo.GetUserById(1))
                .Returns(user)
                .Verifiable();

            _eventRepositoryMock?.Setup(repo => repo.GetEventsAsync(
                It.Is<LocationDetail>(ld => ld != null),
                It.IsAny<double?>(),
                It.Is<UserProfile>(up => up.UserId == 1),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.IsAny<int?>(),
                It.IsAny<EventStatus?>(),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events.AsEnumerable(), totalCount))
                .Verifiable();

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError, "Expected no error, but an error was returned: {0}", result.Errors?.FirstOrDefault().Description);
            Assert.IsNotNull(result.Value, "result.Value is null");
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            Assert.IsNotNull(result.Value.Events, "result.Value.Events is null");
            Assert.IsNotNull(result.Value.Events.Data, "result.Value.Events.Data is null");
            Assert.IsFalse(result.Value.Events.Data.Any());
            Assert.AreEqual(0, result.Value.Events.TotalCount);
            Assert.AreEqual(query.PageNumber, result.Value.Events.PageNumber);
            Assert.AreEqual(query.PageSize, result.Value.Events.PageSize);
            _userRepositoryMock?.Verify();
            _eventRepositoryMock?.Verify();
            _loggerMock?.Verify(x => x.LogInfo($"Fetched {totalCount} events for user {user.Id}. Page {query.PageNumber} has {events.Count} items."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_NullEventsList_ReturnsEmptyPaginatedResponse()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                PageNumber: 1,
                PageSize: 5);

            var user = CreateValidUser();
            var totalCount = 0;

            _userRepositoryMock?.Setup(repo => repo.GetUserById(1))
                .Returns(user)
                .Verifiable();

            _eventRepositoryMock?.Setup(repo => repo.GetEventsAsync(
                It.Is<LocationDetail>(ld => ld != null),
                It.IsAny<double?>(),
                It.Is<UserProfile>(up => up.UserId == 1),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.IsAny<int?>(),
                It.IsAny<EventStatus?>(),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((null as IEnumerable<Events>, totalCount))
                .Verifiable();

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError, "Expected no error, but an error was returned: {0}", result.Errors?.FirstOrDefault().Description);
            Assert.IsNotNull(result.Value, "result.Value is null");
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            Assert.IsNotNull(result.Value.Events, "result.Value.Events is null");
            Assert.IsNotNull(result.Value.Events.Data, "result.Value.Events.Data is null");
            Assert.IsFalse(result.Value.Events.Data.Any());
            Assert.AreEqual(0, result.Value.Events.TotalCount);
            Assert.AreEqual(query.PageNumber, result.Value.Events.PageNumber);
            Assert.AreEqual(query.PageSize, result.Value.Events.PageSize);
            _userRepositoryMock?.Verify();
            _eventRepositoryMock?.Verify();
            _loggerMock?.Verify(x => x.LogInfo($"Fetched {totalCount} events for user {user.Id}. Page {query.PageNumber} has {0} items."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            _userRepositoryMock?.Setup(repo => repo.GetUserById(1)).Returns((User?)null);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.Errors[0]);
            _loggerMock?.Verify(x => x.LogWarn("User not found"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_UserProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            var user = new User { Id = 1, UserProfile = null };
            _userRepositoryMock?.Setup(repo => repo.GetUserById(1)).Returns(user);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.ProfileNotFound, result.Errors[0]);
            _loggerMock?.Verify(x => x.LogWarn($"UserProfile not found for user {user.Id}"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_LocationNotConfigured_ReturnsLocationNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(UserId: 1);
            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = null
                }
            };
            _userRepositoryMock?.Setup(repo => repo.GetUserById(1)).Returns(user);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Profile.LocationNotFound", result.Errors[0].Code);
            Assert.AreEqual("User location is not configured.", result.Errors[0].Description);
            _loggerMock?.Verify(x => x.LogWarn($"User location not found for user {user.Id}"), Times.Once());
        }

        private User CreateValidUser()
        {
            var passions = new List<string> { "music", "comedy" };
            return new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    UserId = 1,
                    PassionsJson = JsonConvert.SerializeObject(passions),
                    Gender = new Gender { GenderType = "Male" },
                    Ethnicity = new Ethnicity { EthnicityType = "WhiteOrCaucasian" },
                    Location = new Location
                    {
                        Lat = 40.7128,
                        Lng = -74.0060,
                        IsVisible = true,
                        LocationDetail = new LocationDetail
                        {
                            Results = new List<LocationResult>
                            {
                                new LocationResult
                                {
                                    Geometry = new Geometry
                                    {
                                        Location = new Locationn { Lat = 40.7128, Lng = -74.0060 },
                                        LocationType = "APPROXIMATE"
                                    },
                                    AddressComponents = new List<AddressComponent>
                                    {
                                        new AddressComponent
                                        {
                                            LongName = "New York",
                                            ShortName = "NY",
                                            Types = new List<string> { "locality" }
                                        }
                                    },
                                    FormattedAddress = "New York, NY, USA",
                                    PlaceId = "ChIJOwg_06VPwokRYv534QaPC8g",
                                    Types = new List<string> { "locality", "political" }
                                }
                            },
                            Status = "OK",
                            LocationId = 1,
                            Location = new Location
                            {
                                Lat = 40.7128,
                                Lng = -74.0060,
                                IsVisible = true
                            }
                        }
                    },
                    DOB = DateTime.UtcNow.AddYears(-30),
                    HaveKids = new HaveKids { HaveKidsType = "No" },
                    WantKids = new WantKids { WantKidsType = "Maybe" }
                }
            };
        }

        private Events CreateValidEvent()
        {
            return new Events
            {
                Id = 1,
                EventTitle = "Test Event",
                EventDescription = "A test event",
                EventType = EventType.Live,
                EventCategory = EventCategory.Free,
                Status = EventStatus.Upcoming,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                UserId = 2,
                Location = new Location
                {
                    Lat = 40.7128,
                    Lng = -74.0060,
                    IsVisible = true,
                    LocationDetail = new LocationDetail
                    {
                        Results = new List<LocationResult>
                        {
                            new LocationResult
                            {
                                Geometry = new Geometry
                                {
                                    Location = new Locationn { Lat = 40.7128, Lng = -74.0060 },
                                    LocationType = "APPROXIMATE"
                                },
                                FormattedAddress = "New York, NY, USA"
                            }
                        },
                        Status = "OK"
                    }
                },
                EventCriteria = new EventCriteria
                {
                    EventType = Event_Type.Comedy,
                    Gender = GenderType.Male,
                    Race = "WhiteOrCaucasian"
                },
                Capacity = 100,
                MinAge = 18,
                MaxAge = 99,
                SponsoredEvent = false,
                IsFlagged = false,
                InviteesException = false
            };
        }
    }
}