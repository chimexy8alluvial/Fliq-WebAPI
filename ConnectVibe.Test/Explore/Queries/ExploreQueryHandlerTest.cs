using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common.Services;
using Fliq.Application.Explore.Queries;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Moq;

namespace Fliq.Test.Explore.Queries
{
    [TestClass]
    public class ExploreQueryHandlerTest
    {
        private ExploreQueryHandler? _handler;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IProfileRepository>? _profileRepositoryMock;
        private Mock<IProfileMatchingService>? _profileMatchingServiceMock;
        private Mock<ILoggerManager>? _loggerMock;

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
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns((User?)null);
            var query = new ExploreQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(1, 10) // Provide valid PaginationRequest
            );

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.User.UserNotFound));
            _loggerMock?.Verify(x => x.LogWarn("User not found"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_UserProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var user = new User { Id = 1, UserProfile = null };
            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            var query = new ExploreQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(1, 10) // Provide valid PaginationRequest
            );

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Profile.ProfileNotFound));
            _loggerMock?.Verify(x => x.LogWarn($"UserProfile not found for user {user.Id}"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ProfilesFetchedSuccessfully_ReturnsExploreResult()
        {
            // Arrange
            var user = new User { Id = 1, UserProfile = new UserProfile() };
            var profiles = new List<UserProfile> { new UserProfile(), new UserProfile() };
            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            _profileMatchingServiceMock?.Setup(x => x.GetMatchedProfilesAsync(user, It.IsAny<ExploreQuery>()))
                                       .ReturnsAsync(profiles);
            _profileRepositoryMock?.Setup(x => x.GetProfileByUserId(user.Id)).Returns(profiles.First());
            var query = new ExploreQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(1, 10) // Provide valid PaginationRequest
            );

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(profiles.Count, result.Value.UserProfiles.TotalCount);
            _loggerMock?.Verify(x => x.LogInfo($"Successfully fetched {profiles.Count} profiles for user."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_NoProfilesFound_ReturnsEmptyExploreResult()
        {
            // Arrange
            var user = new User { Id = 1, UserProfile = new UserProfile() };
            var profiles = new List<UserProfile>();
            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            _profileMatchingServiceMock?.Setup(x => x.GetMatchedProfilesAsync(user, It.IsAny<ExploreQuery>()))
                                       .ReturnsAsync(profiles);
            _profileRepositoryMock?.Setup(x => x.GetProfileByUserId(user.Id)).Returns(user.UserProfile);
            var query = new ExploreQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(1, 10) // Provide valid PaginationRequest
            );

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Value.UserProfiles.TotalCount);
            _loggerMock?.Verify(x => x.LogInfo($"Successfully fetched 0 profiles for user."), Times.Once());
        }
    }
}