using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Application.MatchedProfile.Queries;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Moq;

namespace Fliq.Test.MatchProfile.Queries
{
    [TestClass]
    public class GetRecentUsersMatchQueryHandlerTest
    {
        private Mock<IMatchProfileRepository>? _mockMatchRepository;
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private GetRecentUsersMatchQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMatchRepository = new Mock<IMatchProfileRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();

            _handler = new GetRecentUsersMatchQueryHandler(
                _mockMatchRepository.Object,
                _mockLoggerManager.Object,
                _mockUserRepository.Object
            );
        }

        [TestMethod]
        public async Task Handle_AdminUserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var query = new GetRecentUsersMatchQuery(1, 2, 5);
            _mockUserRepository?.Setup(repo => repo.GetUserById(1)).Returns((User)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_AdminUserNotAuthorized_ReturnsUnauthorizedError()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 3 }; // Not an Admin (1 or 2)
            _mockUserRepository?.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            var query = new GetRecentUsersMatchQuery(1, 2, 5);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UnauthorizedUser, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            _mockUserRepository?.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository?.Setup(repo => repo.GetUserById(2)).Returns((User)null);
            var query = new GetRecentUsersMatchQuery(1, 2, 5);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_LimitExceedsMax_ReturnsCappedResults()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var user = new User { Id = 2, RoleId = 2 };
            var matches = new List<GetRecentUserMatchResult>
        {
            new GetRecentUserMatchResult(32,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(33,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(34,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(35,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(36,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(37,"Josh", "Bellion", "image/url"),
             new GetRecentUserMatchResult(32,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(33,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(34,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(35,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(36,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(37,"Josh", "Bellion", "image/url")
            // More than limit
        };

            _mockUserRepository?.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository?.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository?.Setup(repo => repo.GetRecentMatchesAsync(2, 10))
                .ReturnsAsync(matches.Take(10).ToList());

            var query = new GetRecentUsersMatchQuery(1, 2, 15); // Exceeding max limit

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(10, result.Value.Count);
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsRecentMatches()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 2 };
            var user = new User { Id = 2, RoleId = 2 };
            var matches = new List<GetRecentUserMatchResult>
        {
            new GetRecentUserMatchResult(33,"Josh", "Bellion", "image/url"),
            new GetRecentUserMatchResult(34,"Tomi", "Thomas", "image/url"),
            new GetRecentUserMatchResult(35,"Frank", "Bellion", "image/url")
        };

            _mockUserRepository?.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository?.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository?.Setup(repo => repo.GetRecentMatchesAsync(2, 3))
                .ReturnsAsync(matches);

            var query = new GetRecentUsersMatchQuery(1, 2, 3);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(3, result.Value.Count);
        }

        [TestMethod]
        public async Task Handle_NoRecentMatches_ReturnsEmptyList()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var user = new User { Id = 2, RoleId = 2 };

            _mockUserRepository?.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository?.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository?.Setup(repo => repo.GetRecentMatchesAsync(2, 5))
                .ReturnsAsync([]);

            var query = new GetRecentUsersMatchQuery(1, 2, 5);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Value.Count);
        }
    
    }
}
