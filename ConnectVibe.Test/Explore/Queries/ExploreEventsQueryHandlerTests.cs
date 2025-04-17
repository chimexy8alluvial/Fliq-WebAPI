using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Queries;
using Fliq.Contracts.Explore;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Tests.Explore.Queries
{
    [TestClass]
    public class ExploreEventsQueryHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IEventRepository> _eventRepositoryMock = null!;
        private Mock<ILoggerManager> _loggerMock = null!;
        private ExploreEventsQueryHandler _handler = null!;
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
        public async Task Handle_ValidQueryWithReviewsAndMinRating_ReturnsPaginatedEventsWithFilteredReviewsAndCreatedBy()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: 10.0,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatedBy: "John",
                EventTitle: "Test",
                Status: EventStatus.Upcoming,
                IncludeReviews: true,
                MinRating: 4,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));

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

            var events = new List<EventWithDisplayName>
            {
                new EventWithDisplayName
                {
                    Id = 1,
                    EventTitle = "Test Event",
                    EventDescription = "A fun event",
                    EventType = "Live",
                    EventCategory = "Free",
                    Status = "Upcoming",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CreatedBy = "John Doe",
                    Location = new LocationDto
                    {
                        Lat = 40.7128,
                        Lng = -74.0060,
                        IsVisible = true,
                        LocationDetail = new LocationDetailDto
                        {
                            Status = "OK",
                            Results = new List<LocationResultDto>
                            {
                                new LocationResultDto
                                {
                                    FormattedAddress = "123 Main St",
                                    Geometry = new GeometryDto
                                    {
                                        Location = new LocationnDto { Lat = 40.7128, Lng = -74.0060 },
                                        LocationType = "APPROXIMATE"
                                    },
                                    AddressComponents = new List<object>(),
                                    Types = new List<string>(),
                                    PlaceId = ""
                                }
                            }
                        }
                    },
                    EventCriteria = new EventCriteriaDto { EventType = "Comedy", Gender = "Male", Race = "Asian" },
                    Reviews = new List<EventReviewDto>
                    {
                        new EventReviewDto { UserId = 1, EventId = 1, Rating = 4, Comments = "Great event!" },
                        new EventReviewDto { UserId = 2, EventId = 1, Rating = 5, Comments = "Amazing experience!" }
                    }
                }
            };
            var totalCount = 10;

            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock.Setup(repo => repo.GetEventsAsync(
                It.Is<LocationDetail>(ld => ld.Location.Lat == 40.7128 && ld.Location.Lng == -74.0060),
                It.Is<double?>(d => d == 10.0),
                It.Is<UserProfile>(up => up.Gender.GenderType == "Male" && up.Ethnicity!.EthnicityType == "Asian"),
                It.Is<EventCategory?>(c => c == EventCategory.Free),
                It.Is<EventType?>(t => t == EventType.Live),
                It.Is<string>(cb => cb == "John"),
                 It.Is<string>(et => string.Equals(et, "Test", StringComparison.OrdinalIgnoreCase)),
                It.Is<EventStatus?>(s => s == EventStatus.Upcoming),
                It.Is<bool?>(ir => ir == true),
                It.Is<int?>(mr => mr == 4),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.AreEqual(totalCount, response.TotalCount);
            Assert.AreEqual(query.PaginationRequest.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PaginationRequest.PageSize, response.PageSize);
            Assert.AreEqual(1, response.Data.Count());
            var eventData = response.Data.First();
            Assert.AreEqual("John Doe", eventData.CreatedBy);
            Assert.IsNull(eventData.GetType().GetProperty("UserId")); // Verify UserId is not present
            Assert.AreEqual("Comedy", eventData.EventCriteria?.EventType);
            Assert.AreEqual(2, eventData.Reviews.Count);
            Assert.IsTrue(eventData.Reviews.All(r => r.Rating >= 4));
            Assert.AreEqual(4, eventData.Reviews[0].Rating);
            Assert.AreEqual("Great event!", eventData.Reviews[0].Comments);
            Assert.AreEqual(5, eventData.Reviews[1].Rating);
            Assert.AreEqual("Amazing experience!", eventData.Reviews[1].Comments);
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetching events with reviews") && s.Contains("MinRating=4"))), Times.Once());
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetched 10 events with reviews"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithoutReviews_ReturnsPaginatedEventsWithEmptyReviewsAndCreatedBy()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: 10.0,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatedBy: "John",
                EventTitle: "Test",
                Status: EventStatus.Upcoming,
                IncludeReviews: false,
                MinRating: null,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));

            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };

            var events = new List<EventWithDisplayName>
            {
                new EventWithDisplayName
                {
                    Id = 1,
                    EventTitle = "Test Event",
                    EventDescription = "A fun event",
                    EventType = "Live",
                    EventCategory = "Free",
                    Status = "Upcoming",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CreatedBy = "John Doe",
                    Location = new LocationDto
                    {
                        Lat = 40.7128,
                        Lng = -74.0060,
                        IsVisible = true,
                        LocationDetail = new LocationDetailDto
                        {
                            Status = "OK",
                            Results = new List<LocationResultDto>
                            {
                                new LocationResultDto
                                {
                                    FormattedAddress = "123 Main St",
                                    Geometry = new GeometryDto
                                    {
                                        Location = new LocationnDto { Lat = 40.7128, Lng = -74.0060 },
                                        LocationType = "APPROXIMATE"
                                    },
                                    AddressComponents = new List<object>(),
                                    Types = new List<string>(),
                                    PlaceId = ""
                                }
                            }
                        }
                    },
                    EventCriteria = new EventCriteriaDto { EventType = "Comedy", Gender = "Male", Race = "Asian" },
                    Reviews = new List<EventReviewDto>()
                }
            };
            var totalCount = 5;

            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.Is<string>(cb => cb == "John"),
                It.Is<string>(et => string.Equals(et, "Test", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<EventStatus?>(),
                It.Is<bool?>(ir => ir == false),
                It.Is<int?>(mr => mr == null),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.AreEqual(totalCount, response.TotalCount);
            Assert.AreEqual(query.PaginationRequest.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PaginationRequest.PageSize, response.PageSize);
            Assert.AreEqual(1, response.Data.Count());
            var eventData = response.Data.First();
            Assert.AreEqual("John Doe", eventData.CreatedBy);
            Assert.IsNull(eventData.GetType().GetProperty("UserId")); // Verify UserId is not present
            Assert.AreEqual("Comedy", eventData.EventCriteria?.EventType);
            Assert.AreEqual(0, eventData.Reviews.Count);
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetching events without reviews") && s.Contains("CreatedBy=John"))), Times.Once());
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetched 5 events without reviews"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithNullIncludeReviewsAndMinRating_ReturnsPaginatedEventsWithEmptyReviewsAndCreatedBy()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: 10.0,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatedBy: "John",
                EventTitle: "Test",
                Status: EventStatus.Upcoming,
                IncludeReviews: null,
                MinRating: null,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));

            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };

            var events = new List<EventWithDisplayName>
            {
                new EventWithDisplayName
                {
                    Id = 1,
                    EventTitle = "Test Event",
                    EventDescription = "A fun event",
                    EventType = "Live",
                    EventCategory = "Free",
                    Status = "Upcoming",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CreatedBy = "John Doe",
                    Location = new LocationDto
                    {
                        Lat = 40.7128,
                        Lng = -74.0060,
                        IsVisible = true,
                        LocationDetail = new LocationDetailDto
                        {
                            Status = "OK",
                            Results = new List<LocationResultDto>
                            {
                                new LocationResultDto
                                {
                                    FormattedAddress = "123 Main St",
                                    Geometry = new GeometryDto
                                    {
                                        Location = new LocationnDto { Lat = 40.7128, Lng = -74.0060 },
                                        LocationType = "APPROXIMATE"
                                    },
                                    AddressComponents = new List<object>(),
                                    Types = new List<string>(),
                                    PlaceId = ""
                                }
                            }
                        }
                    },
                    EventCriteria = new EventCriteriaDto { EventType = "Comedy", Gender = "Male", Race = "Asian" },
                    Reviews = new List<EventReviewDto>()
                }
            };
            var totalCount = 5;

            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.Is<string>(cb => cb == "John"),
                It.Is<string>(et => string.Equals(et, "Test", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<EventStatus?>(),
                It.Is<bool?>(ir => ir == null),
                It.Is<int?>(mr => mr == null),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.AreEqual(totalCount, response.TotalCount);
            Assert.AreEqual(query.PaginationRequest.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PaginationRequest.PageSize, response.PageSize);
            Assert.AreEqual(1, response.Data.Count());
            var eventData = response.Data.First();
            Assert.AreEqual("John Doe", eventData.CreatedBy);
            Assert.IsNull(eventData.GetType().GetProperty("UserId")); // Verify UserId is not present
            Assert.AreEqual("Comedy", eventData.EventCriteria?.EventType);
            Assert.AreEqual(0, eventData.Reviews.Count);
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetching events without reviews") && s.Contains("CreatedBy=John"))), Times.Once());
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetched 5 events without reviews"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithMinRatingNoReviews_ReturnsPaginatedEventsWithEmptyReviews()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: 10.0,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatedBy: "John",
                 EventTitle: "Test",
                Status: EventStatus.Upcoming,
                IncludeReviews: true,
                MinRating: 5,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));

            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };

            var events = new List<EventWithDisplayName>
            {
                new EventWithDisplayName
                {
                    Id = 1,
                    EventTitle = "Test Event",
                    EventDescription = "A fun event",
                    EventType = "Live",
                    EventCategory = "Free",
                    Status = "Upcoming",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CreatedBy = "John Doe",
                    Location = new LocationDto
                    {
                        Lat = 40.7128,
                        Lng = -74.0060,
                        IsVisible = true,
                        LocationDetail = new LocationDetailDto
                        {
                            Status = "OK",
                            Results = new List<LocationResultDto>
                            {
                                new LocationResultDto
                                {
                                    FormattedAddress = "123 Main St",
                                    Geometry = new GeometryDto
                                    {
                                        Location = new LocationnDto { Lat = 40.7128, Lng = -74.0060 },
                                        LocationType = "APPROXIMATE"
                                    },
                                    AddressComponents = new List<object>(),
                                    Types = new List<string>(),
                                    PlaceId = ""
                                }
                            }
                        }
                    },
                    EventCriteria = new EventCriteriaDto { EventType = "Comedy", Gender = "Male", Race = "Asian" },
                    Reviews = new List<EventReviewDto>() // No reviews meet MinRating = 5
                }
            };
            var totalCount = 5;

            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.Is<string>(cb => cb == "John"),
                 It.Is<string>(et => string.Equals(et, "Test", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<EventStatus?>(),
                It.Is<bool?>(ir => ir == true),
                It.Is<int?>(mr => mr == 5),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.AreEqual(totalCount, response.TotalCount);
            Assert.AreEqual(query.PaginationRequest.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PaginationRequest.PageSize, response.PageSize);
            Assert.AreEqual(1, response.Data.Count());
            var eventData = response.Data.First();
            Assert.AreEqual("John Doe", eventData.CreatedBy);
            Assert.IsNull(eventData.GetType().GetProperty("UserId")); // Verify UserId is not present
            Assert.AreEqual("Comedy", eventData.EventCriteria?.EventType);
            Assert.AreEqual(0, eventData.Reviews.Count);
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetching events with reviews") && s.Contains("MinRating=5"))), Times.Once());
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetched 5 events with reviews"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithInvalidPagination_CapsPageSizeAtFive()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: 10.0,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatedBy: "John",
                 EventTitle: "Test",
                Status: EventStatus.Upcoming,
                IncludeReviews: false,
                MinRating: null,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 10));

            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };

            var events = new List<EventWithDisplayName>
            {
                new EventWithDisplayName
                {
                    Id = 1,
                    EventTitle = "Test Event",
                    EventDescription = "A fun event",
                    EventType = "Live",
                    EventCategory = "Free",
                    Status = "Upcoming",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CreatedBy = "John Doe",
                    Location = new LocationDto
                    {
                        Lat = 40.7128,
                        Lng = -74.0060,
                        IsVisible = true,
                        LocationDetail = new LocationDetailDto
                        {
                            Status = "OK",
                            Results = new List<LocationResultDto>
                            {
                                new LocationResultDto
                                {
                                    FormattedAddress = "123 Main St",
                                    Geometry = new GeometryDto
                                    {
                                        Location = new LocationnDto { Lat = 40.7128, Lng = -74.0060 },
                                        LocationType = "APPROXIMATE"
                                    },
                                    AddressComponents = new List<object>(),
                                    Types = new List<string>(),
                                    PlaceId = ""
                                }
                            }
                        }
                    },
                    EventCriteria = new EventCriteriaDto { EventType = "Comedy", Gender = "Male", Race = "Asian" },
                    Reviews = new List<EventReviewDto>()
                }
            };
            var totalCount = 5;

            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.Is<string>(cb => cb == "John"),
                 It.Is<string>(et => string.Equals(et, "Test", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<EventStatus?>(),
                It.Is<bool?>(ir => ir == false),
                It.Is<int?>(mr => mr == null),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.AreEqual(totalCount, response.TotalCount);
            Assert.AreEqual(query.PaginationRequest.PageNumber, response.PageNumber);
            Assert.AreEqual(5, response.PageSize);
            Assert.AreEqual(1, response.Data.Count());
            var eventData = response.Data.First();
            Assert.AreEqual("John Doe", eventData.CreatedBy);
            Assert.IsNull(eventData.GetType().GetProperty("UserId")); // Verify UserId is not present
            Assert.AreEqual("Comedy", eventData.EventCriteria?.EventType);
            Assert.AreEqual(0, eventData.Reviews.Count);
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetching events without reviews") && s.Contains("PageSize=5"))), Times.Once());
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetched 5 events without reviews"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithMissingCreator_ReturnsPaginatedEventsWithDefaultCreatedBy()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                MaxDistanceKm: 10.0,
                Category: EventCategory.Free,
                EventType: EventType.Live,
                CreatedBy: null,
                EventTitle: "Test",
                Status: EventStatus.Upcoming,
                IncludeReviews: false,
                MinRating: null,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));

            var user = new User
            {
                Id = 1,
                UserProfile = new UserProfile
                {
                    Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
                }
            };

            var events = new List<EventWithDisplayName>
            {
                new EventWithDisplayName
                {
                    Id = 1,
                    EventTitle = "Test Event",
                    EventDescription = "A fun event",
                    EventType = "Live",
                    EventCategory = "Free",
                    Status = "Upcoming",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CreatedBy = "Unknown",
                    Location = new LocationDto
                    {
                        Lat = 40.7128,
                        Lng = -74.0060,
                        IsVisible = true,
                        LocationDetail = new LocationDetailDto
                        {
                            Status = "OK",
                            Results = new List<LocationResultDto>
                            {
                                new LocationResultDto
                                {
                                    FormattedAddress = "123 Main St",
                                    Geometry = new GeometryDto
                                    {
                                        Location = new LocationnDto { Lat = 40.7128, Lng = -74.0060 },
                                        LocationType = "APPROXIMATE"
                                    },
                                    AddressComponents = new List<object>(),
                                    Types = new List<string>(),
                                    PlaceId = ""
                                }
                            }
                        }
                    },
                    EventCriteria = new EventCriteriaDto { EventType = "Comedy", Gender = "Male", Race = "Asian" },
                    Reviews = new List<EventReviewDto>()
                }
            };
            var totalCount = 5;

            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _eventRepositoryMock.Setup(repo => repo.GetEventsAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.Is<string>(cb => cb == null),
                It.Is<string>(et => string.Equals(et, "Test", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<EventStatus?>(),
                It.Is<bool?>(ir => ir == false),
                It.Is<int?>(mr => mr == null),
                It.Is<PaginationRequest>(p => p.PageNumber == 1 && p.PageSize == 5)))
                .ReturnsAsync((events, totalCount));

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreEventsResult));
            var response = result.Value.Events;
            Assert.AreEqual(totalCount, response.TotalCount);
            Assert.AreEqual(query.PaginationRequest.PageNumber, response.PageNumber);
            Assert.AreEqual(query.PaginationRequest.PageSize, response.PageSize);
            Assert.AreEqual(1, response.Data.Count());
            var eventData = response.Data.First();
            Assert.AreEqual("Unknown", eventData.CreatedBy);
            Assert.IsNull(eventData.GetType().GetProperty("UserId")); // Verify UserId is not present
            Assert.AreEqual("Comedy", eventData.EventCriteria?.EventType);
            Assert.AreEqual(0, eventData.Reviews.Count);
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetching events without reviews"))), Times.Once());
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Fetched 5 events without reviews"))), Times.Once());
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns((User)null!);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.Errors[0]);
            _loggerMock.Verify(l => l.LogWarn(It.Is<string>(s => s.Contains("User not found"))), Times.Once());
        }
    }
}