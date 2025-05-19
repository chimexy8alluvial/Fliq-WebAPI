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
                PaginationRequest: new PaginationRequest(1, 10),
                FilterByEvent: true
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
                PaginationRequest: new PaginationRequest(1, 10),
                FilterByEvent: true
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
            var paginationResponse = new PaginationResponse<UserProfile>(
                data: profiles,
                totalCount: 100,
                pageNumber: 1,
                pageSize: 10
            );
            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            _profileMatchingServiceMock?.Setup(x => x.GetMatchedProfilesAsync(user, It.Is<ExploreQuery>(q => q.FilterByEvent == true)))
                                       .ReturnsAsync(paginationResponse);
            var query = new ExploreQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(1, 10),
                FilterByDating: true,
                FilterByFriendship: false,
                FilterByEvent: true
            );

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(profiles.Count, result.Value.UserProfiles.Data.Count());
            Assert.AreEqual(100, result.Value.UserProfiles.TotalCount);
            Assert.AreEqual(1, result.Value.UserProfiles.PageNumber);
            Assert.AreEqual(10, result.Value.UserProfiles.PageSize);
            Assert.AreEqual(10, result.Value.UserProfiles.TotalPages);
            Assert.IsTrue(result.Value.UserProfiles.HasNextPage);
            Assert.IsFalse(result.Value.UserProfiles.HasPreviousPage);
            _loggerMock?.Verify(x => x.LogInfo($"Successfully fetched {profiles.Count} profiles for user."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_NoProfilesFound_ReturnsEmptyExploreResult()
        {
            // Arrange
            var user = new User { Id = 1, UserProfile = new UserProfile() };
            var profiles = new List<UserProfile>();
            var paginationResponse = new PaginationResponse<UserProfile>(
                data: profiles,
                totalCount: 0,
                pageNumber: 1,
                pageSize: 10
            );
            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            _profileMatchingServiceMock?.Setup(x => x.GetMatchedProfilesAsync(user, It.Is<ExploreQuery>(q => q.FilterByEvent == true)))
                                       .ReturnsAsync(paginationResponse);
            var query = new ExploreQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(1, 10),
                FilterByDating: false,
                FilterByFriendship: true,
                FilterByEvent: true
            );

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(0, result.Value.UserProfiles.Data.Count());
            Assert.AreEqual(0, result.Value.UserProfiles.TotalCount);
            Assert.AreEqual(1, result.Value.UserProfiles.PageNumber);
            Assert.AreEqual(10, result.Value.UserProfiles.PageSize);
            Assert.AreEqual(0, result.Value.UserProfiles.TotalPages);
            Assert.IsFalse(result.Value.UserProfiles.HasNextPage);
            Assert.IsFalse(result.Value.UserProfiles.HasPreviousPage);
            _loggerMock?.Verify(x => x.LogInfo($"Successfully fetched 0 profiles for user."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_PaginationMetadata_CorrectlyCalculated()
        {
            // Arrange
            var user = new User { Id = 1, UserProfile = new UserProfile() };
            var profiles = new List<UserProfile> { new UserProfile(), new UserProfile(), new UserProfile() };
            var paginationResponse = new PaginationResponse<UserProfile>(
                data: profiles,
                totalCount: 25,
                pageNumber: 2,
                pageSize: 10
            );
            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            _profileMatchingServiceMock?.Setup(x => x.GetMatchedProfilesAsync(user, It.Is<ExploreQuery>(q => q.FilterByEvent == true)))
                                       .ReturnsAsync(paginationResponse);
            var query = new ExploreQuery(
                UserId: 1,
                PaginationRequest: new PaginationRequest(2, 10),
                FilterByDating: true,
                FilterByFriendship: false,
                FilterByEvent: true
            );

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(profiles.Count, result.Value.UserProfiles.Data.Count());
            Assert.AreEqual(25, result.Value.UserProfiles.TotalCount);
            Assert.AreEqual(2, result.Value.UserProfiles.PageNumber);
            Assert.AreEqual(10, result.Value.UserProfiles.PageSize);
            Assert.AreEqual(3, result.Value.UserProfiles.TotalPages);
            Assert.IsTrue(result.Value.UserProfiles.HasNextPage);
            Assert.IsTrue(result.Value.UserProfiles.HasPreviousPage);
            _loggerMock?.Verify(x => x.LogInfo($"Successfully fetched {profiles.Count} profiles for user."), Times.Once());
        }
    }
}