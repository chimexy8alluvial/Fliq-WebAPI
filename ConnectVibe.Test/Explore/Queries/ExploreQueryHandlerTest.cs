using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Explore.Queries;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Test.Explore.Queries
{
    [TestClass]
    public class ExploreQueryHandlerTest
    {
        private ExploreQueryHandler _handler;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IProfileRepository> _profileRepositoryMock;
        private Mock<ILoggerManager> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _loggerMock = new Mock<ILoggerManager>();

            _handler = new ExploreQueryHandler(
                _httpContextAccessorMock.Object,
                _userRepositoryMock.Object,
                _profileRepositoryMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetUserById(It.IsAny<int>())).Returns((User?)null);

            var query = new ExploreQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.User.UserNotFound));
            _loggerMock.Verify(x => x.LogWarn("User not found"), Times.Once);
        }

        [TestMethod]
        public async Task Handle_UserProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var user = new User { Id = 1, UserProfile = null };
            _userRepositoryMock.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);

            var query = new ExploreQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Profile.ProfileNotFound));
            _loggerMock.Verify(x => x.LogWarn($"UserProfile not found for user {user.Id}"), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ProfilesFetchedSuccessfully_ReturnsExploreResult()
        {
            // Arrange
            var user = new User { Id = 1, UserProfile = new UserProfile() };
            var profiles = new List<UserProfile> { new UserProfile(), new UserProfile() };

            _userRepositoryMock.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            _profileRepositoryMock.Setup(x => x.GetProfilesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), It.IsAny<bool?>()))
                                  .ReturnsAsync(profiles);

            var query = new ExploreQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(profiles.Count, result.Value.UserProfiles.TotalCount);
            _loggerMock.Verify(x => x.LogInfo($"Successfully fetched {profiles.Count} profiles for user."), Times.Once);
        }

        [TestMethod]
        public async Task Handle_NoProfilesFound_ReturnsEmptyExploreResult()
        {
            // Arrange
            var user = new User { Id = 1, UserProfile = new UserProfile() };
            var profiles = new List<UserProfile>();

            _userRepositoryMock.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            _profileRepositoryMock.Setup(x => x.GetProfilesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), It.IsAny<bool?>()))
                                  .ReturnsAsync(profiles);

            var query = new ExploreQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Value.UserProfiles.TotalCount);
            _loggerMock.Verify(x => x.LogInfo($"Successfully fetched 0 profiles for user."), Times.Once);
        }
    }
}
