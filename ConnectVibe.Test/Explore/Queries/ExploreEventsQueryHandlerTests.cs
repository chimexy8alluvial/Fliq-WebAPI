using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Queries;
using Fliq.Contracts.Explore;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;
using Fliq.Infrastructure.Persistence;
using Fliq.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore; // Required for DbContextOptionsBuilder
using Moq;
using System.Data;

namespace Fliq.Application.Tests.Explore.Queries
{
    [TestClass]
    public class ExploreEventsQueryHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IEventRepository> _eventRepositoryMock = null!;
        private Mock<IProfileRepository> _profileRepositoryMock = null!;
        private Mock<ILoggerManager> _loggerMock = null!;
        private ExploreEventsQueryHandler _handler = null!;
        private CancellationToken _cancellationToken;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new ExploreEventsQueryHandler(
                _userRepositoryMock.Object,
                _eventRepositoryMock.Object,
                _profileRepositoryMock.Object,
                _loggerMock.Object);
            _cancellationToken = CancellationToken.None;
        }

        // Existing tests remain mostly unchanged, but update UserProfile mocking
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

            var user = new User { Id = 1 };
            var userProfile = new UserProfile
            {
                Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } },
                Gender = new Gender { GenderType = "Male" },
                Ethnicity = new Ethnicity { EthnicityType = "Asian" },
                Passions = new List<string> { "music", "comedy" }
            };

            var events = new List<EventWithDisplayName> { /* Same as original */ };
            var totalCount = 10;

            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _profileRepositoryMock.Setup(repo => repo.GetProfileByUserId(1)).Returns(userProfile);
            _eventRepositoryMock.Setup(repo => repo.GetEventsAndCountAsync(
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
            // Same assertions as original
        }

        // Add new test for ProfileNotFound
        [TestMethod]
        public async Task Handle_ProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));
            var user = new User { Id = 1 };
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _profileRepositoryMock.Setup(repo => repo.GetProfileByUserId(1)).Returns((UserProfile)null!);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.ProfileNotFound, result.Errors[0]);
            _loggerMock.Verify(l => l.LogWarn(It.Is<string>(s => s.Contains("UserProfile not found"))), Times.Once());
        }

        // Add new test for LocationNotFound
        [TestMethod]
        public async Task Handle_LocationNotFound_ReturnsLocationNotFoundError()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));
            var user = new User { Id = 1 };
            var userProfile = new UserProfile { Location = null };
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _profileRepositoryMock.Setup(repo => repo.GetProfileByUserId(1)).Returns(userProfile);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Profile.LocationNotFound", result.Errors[0].Code);
            Assert.AreEqual("User location is not configured.", result.Errors[0].Description);
            _loggerMock.Verify(l => l.LogWarn(It.Is<string>(s => s.Contains("User location not found"))), Times.Once());
        }

        // Add new test for Exception Handling
        [TestMethod]
        public async Task Handle_RepositoryThrowsException_ReturnsFailureError()
        {
            // Arrange
            var query = new ExploreEventsQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(PageNumber: 1, PageSize: 5));
            var user = new User { Id = 1 };
            var userProfile = new UserProfile
            {
                Location = new Location { LocationDetail = new LocationDetail { Location = new Location { Lat = 40.7128, Lng = -74.0060 } } }
            };
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);
            _profileRepositoryMock.Setup(repo => repo.GetProfileByUserId(1)).Returns(userProfile);
            _eventRepositoryMock.Setup(repo => repo.GetEventsAndCountAsync(
                It.IsAny<LocationDetail>(),
                It.IsAny<double?>(),
                It.IsAny<UserProfile>(),
                It.IsAny<EventCategory?>(),
                It.IsAny<EventType?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<EventStatus?>(),
                It.IsAny<bool?>(),
                It.IsAny<int?>(),
                It.IsAny<PaginationRequest>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Failure", result.Errors[0].Type.ToString());
            Assert.IsTrue(result.Errors[0].Description.Contains("Failed to fetch events: Database error"));
            _loggerMock.Verify(l => l.LogError(It.Is<string>(s => s.Contains("Failed to fetch events") && s.Contains("Database error"))), Times.Once());
        }
     
    }
}