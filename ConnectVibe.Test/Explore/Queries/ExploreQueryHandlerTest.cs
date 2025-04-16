using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Common.Services;
using Fliq.Application.Explore.Queries;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Moq;

namespace Fliq.Application.Tests.Explore.Queries
{
    [TestClass]
    public class ExploreQueryHandlerTests
    {
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IProfileRepository>? _profileRepositoryMock;
        private Mock<IProfileMatchingService>? _profileMatchingServiceMock;
        private Mock<ILoggerManager>? _loggerMock;
        private ExploreQueryHandler? _handler;
        private CancellationToken _cancellationToken;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _profileMatchingServiceMock = new Mock<IProfileMatchingService>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new ExploreQueryHandler(
                _userRepositoryMock.Object,
                _profileRepositoryMock.Object,
                _loggerMock.Object,
                _profileMatchingServiceMock.Object);
            _cancellationToken = CancellationToken.None;
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsPaginatedProfiles()
        {
            // Arrange
            var query = new ExploreQuery(
                UserId: 1,
                FilterByEvent: true,
                FilterByDating: false,
                FilterByFriendship: null,
                PaginationRequest: new PaginationRequest(1, 5));

            var user = new User { Id = 1 };
            var userProfile = new UserProfile { UserId = 1 };
            var profiles = new List<UserProfile> { new UserProfile { UserId = 2 } };
            var totalCount = 10;

            _userRepositoryMock?.Setup(repo => repo.GetUserById(1)).Returns(user);
            _profileRepositoryMock?.Setup(repo => repo.GetProfileByUserId(1)).Returns(userProfile);
            _profileMatchingServiceMock?.Setup(service => service.GetMatchedProfilesAsync(user, query))
                .ReturnsAsync(profiles);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreResult));
            Assert.AreEqual(profiles, result.Value.UserProfiles.Data);
            Assert.AreEqual(totalCount, result.Value.UserProfiles.TotalCount);
            Assert.AreEqual(query.PaginationRequest.PageNumber, result.Value.UserProfiles.PageNumber);
            Assert.AreEqual(query.PaginationRequest.PageSize, result.Value.UserProfiles.PageSize);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var query = new ExploreQuery(UserId: 1, PaginationRequest: new PaginationRequest(1, 5));
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns((User)null!);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.Errors[0]);
        }

        [TestMethod]
        public async Task Handle_UserProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var query = new ExploreQuery(UserId: 1, PaginationRequest: new PaginationRequest(1, 5));
            var user = new User { Id = 1 };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);
            _profileRepositoryMock!.Setup(repo => repo.GetProfileByUserId(1)).Returns((UserProfile)null!);

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.ProfileNotFound, result.Errors[0]);
        }

        [TestMethod]
        public async Task Handle_ExceptionThrown_ReturnsFailureError()
        {
            // Arrange
            var query = new ExploreQuery(UserId: 1, PaginationRequest: new PaginationRequest(1, 5));
            var user = new User { Id = 1 };
            var userProfile = new UserProfile { UserId = 1 };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);
            _profileRepositoryMock!.Setup(repo => repo.GetProfileByUserId(1)).Returns(userProfile);
            _profileMatchingServiceMock!.Setup(service => service.GetMatchedProfilesAsync(user, query))
                .ThrowsAsync(new Exception("Profile matching error"));

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual("Failure", result.Errors[0].Type.ToString());
            Assert.IsTrue(result.Errors[0].Description.Contains("Profile matching error"));
        }

        [TestMethod]
        public async Task Handle_NullProfilesList_ReturnsEmptyPaginatedResponse()
        {
            // Arrange
            var query = new ExploreQuery(UserId: 1, PaginationRequest: new PaginationRequest(1, 5));
            var user = new User { Id = 1 };
            var userProfile = new UserProfile { UserId = 1 };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(user);
            _profileRepositoryMock!.Setup(repo => repo.GetProfileByUserId(1)).Returns(userProfile);
            _profileMatchingServiceMock!.Setup(service => service.GetMatchedProfilesAsync(user, query))
                .ReturnsAsync(new List<UserProfile>());

            // Act
            var result = await _handler!.Handle(query, _cancellationToken);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(ExploreResult));
            Assert.IsNotNull(result.Value.UserProfiles.Data);
            Assert.AreEqual(0, result.Value.UserProfiles.TotalCount);
        }
    }
}