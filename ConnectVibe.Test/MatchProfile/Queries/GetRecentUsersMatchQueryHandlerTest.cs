using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Application.MatchedProfile.Queries;
using Fliq.Application.MatchedProfile.Queries.Fliq.Application.MatchedProfile.Queries;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.MatchProfile.Queries
{
    [TestClass]
    public class GetRecentUsersMatchQueryHandlerTest
    {
        private Mock<IMatchProfileRepository>? _mockMatchRepository;
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private Mock<IAuditTrailService>? _mockAuditTrailService;
        private GetRecentUsersMatchQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMatchRepository = new Mock<IMatchProfileRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();
            _mockAuditTrailService = new Mock<IAuditTrailService>();

            _handler = new GetRecentUsersMatchQueryHandler(
                _mockMatchRepository.Object,
                _mockLoggerManager.Object,
                _mockUserRepository.Object,
                _mockAuditTrailService.Object
            );
        }

        [TestMethod]
        public async Task Handle_AdminUserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var query = new GetRecentUsersMatchQuery(1, 2, 5, null);
            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns((User)null!);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
            _mockLoggerManager!.Verify(logger => logger.LogError($"Admin user with ID 1 not found"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_AdminUserNotAuthorized_ReturnsUnauthorizedError()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 3 }; // Not an Admin (1 or 2)
            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            var query = new GetRecentUsersMatchQuery(1, 2, 5, null);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UnauthorizedUser, result.FirstError);
            _mockLoggerManager!.Verify(logger => logger.LogError($"User with ID 1 is not an Admin"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository!.Setup(repo => repo.GetUserById(2)).Returns((User)null!);
            var query = new GetRecentUsersMatchQuery(1, 2, 5, null);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
            _mockLoggerManager!.Verify(logger => logger.LogError($"User with ID 2 not found"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_LimitExceedsMax_ReturnsCappedResults()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var user = new User { Id = 2, RoleId = 2 };
            var matches = new List<GetRecentUserMatchResult>
            {
                new GetRecentUserMatchResult(32, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(33, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(34, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(35, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(36, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(37, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(38, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(39, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(40, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(41, "Josh", "Bellion", "image/url", DateTime.UtcNow)
            };

            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository!.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository!.Setup(repo => repo.GetRecentMatchesAsync(2, 10, null))
                .ReturnsAsync(matches.Take(10).ToList());
            _mockAuditTrailService!.Setup(service => service.LogAuditTrail(It.IsAny<string>(), user))
                .Returns(Task.CompletedTask);

            var query = new GetRecentUsersMatchQuery(1, 2, 15, null);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(10, result.Value.Count);
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Limit reduced from 15 to 10"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Fetching 15 recent matches for UserId=2, Status=Any"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Calling GetRecentMatchesAsync: UserId=2, Limit=10, AcceptedStatus=null"), Times.Once());
            _mockAuditTrailService!.Verify(service => service.LogAuditTrail($"Getting recent user match result for user with ID 2", user), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithAcceptedStatus_ReturnsRecentMatches()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var user = new User { Id = 2, RoleId = 2 };
            var matches = new List<GetRecentUserMatchResult>
            {
                new GetRecentUserMatchResult(33, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(34, "Tomi", "Thomas", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(35, "Frank", "Bellion", "image/url", DateTime.UtcNow)
            };

            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository!.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository!.Setup(repo => repo.GetRecentMatchesAsync(2, 3, (int)MatchRequestStatus.Accepted))
                .ReturnsAsync(matches);
            _mockAuditTrailService!.Setup(service => service.LogAuditTrail(It.IsAny<string>(), user))
                .Returns(Task.CompletedTask);

            var query = new GetRecentUsersMatchQuery(1, 2, 3, MatchRequestStatus.Accepted);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(3, result.Value.Count);
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Fetching 3 recent matches for UserId=2, Status=Accepted"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Calling GetRecentMatchesAsync: UserId=2, Limit=3, AcceptedStatus=0"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"3 recent user matches fetched for UserId=2"), Times.Once());
            _mockAuditTrailService!.Verify(service => service.LogAuditTrail($"Getting recent user match result for user with ID 2", user), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithRejectedStatus_ReturnsRecentMatches()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var user = new User { Id = 2, RoleId = 2 };
            var matches = new List<GetRecentUserMatchResult>
            {
                new GetRecentUserMatchResult(36, "Alice", "Smith", "image/url", DateTime.UtcNow)
            };

            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository!.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository!.Setup(repo => repo.GetRecentMatchesAsync(2, 5, (int)MatchRequestStatus.Rejected))
                .ReturnsAsync(matches);
            _mockAuditTrailService!.Setup(service => service.LogAuditTrail(It.IsAny<string>(), user))
                .Returns(Task.CompletedTask);

            var query = new GetRecentUsersMatchQuery(1, 2, 5, MatchRequestStatus.Rejected);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1, result.Value.Count);
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Fetching 5 recent matches for UserId=2, Status=Rejected"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Calling GetRecentMatchesAsync: UserId=2, Limit=5, AcceptedStatus=1"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"1 recent user matches fetched for UserId=2"), Times.Once());
            _mockAuditTrailService!.Verify(service => service.LogAuditTrail($"Getting recent user match result for user with ID 2", user), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithPendingStatus_ReturnsRecentMatches()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var user = new User { Id = 2, RoleId = 2 };
            var matches = new List<GetRecentUserMatchResult>
            {
                new GetRecentUserMatchResult(37, "Bob", "Jones", "image/url", DateTime.UtcNow)
            };

            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository!.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository!.Setup(repo => repo.GetRecentMatchesAsync(2, 5, (int)MatchRequestStatus.Pending))
                .ReturnsAsync(matches);
            _mockAuditTrailService!.Setup(service => service.LogAuditTrail(It.IsAny<string>(), user))
                .Returns(Task.CompletedTask);

            var query = new GetRecentUsersMatchQuery(1, 2, 5, MatchRequestStatus.Pending);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1, result.Value.Count);
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Fetching 5 recent matches for UserId=2, Status=Pending"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Calling GetRecentMatchesAsync: UserId=2, Limit=5, AcceptedStatus=2"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"1 recent user matches fetched for UserId=2"), Times.Once());
            _mockAuditTrailService!.Verify(service => service.LogAuditTrail($"Getting recent user match result for user with ID 2", user), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidQueryWithNullStatus_ReturnsAllRecentMatches()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var user = new User { Id = 2, RoleId = 2 };
            var matches = new List<GetRecentUserMatchResult>
            {
                new GetRecentUserMatchResult(33, "Josh", "Bellion", "image/url", DateTime.UtcNow),
                new GetRecentUserMatchResult(34, "Tomi", "Thomas", "image/url", DateTime.UtcNow)
            };

            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository!.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository!.Setup(repo => repo.GetRecentMatchesAsync(2, 5, null))
                .ReturnsAsync(matches);
            _mockAuditTrailService!.Setup(service => service.LogAuditTrail(It.IsAny<string>(), user))
                .Returns(Task.CompletedTask);

            var query = new GetRecentUsersMatchQuery(1, 2, 5, null);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(2, result.Value.Count);
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Fetching 5 recent matches for UserId=2, Status=Any"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Calling GetRecentMatchesAsync: UserId=2, Limit=5, AcceptedStatus=null"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"2 recent user matches fetched for UserId=2"), Times.Once());
            _mockAuditTrailService!.Verify(service => service.LogAuditTrail($"Getting recent user match result for user with ID 2", user), Times.Once());
        }

        [TestMethod]
        public async Task Handle_NoRecentMatches_ReturnsEmptyList()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var user = new User { Id = 2, RoleId = 2 };

            _mockUserRepository!.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository!.Setup(repo => repo.GetUserById(2)).Returns(user);
            _mockMatchRepository!.Setup(repo => repo.GetRecentMatchesAsync(2, 5, null))
                .ReturnsAsync(new List<GetRecentUserMatchResult>());
            _mockAuditTrailService!.Setup(service => service.LogAuditTrail(It.IsAny<string>(), user))
                .Returns(Task.CompletedTask);

            var query = new GetRecentUsersMatchQuery(1, 2, 5, null);

            // Act
            var result = await _handler!.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Value.Count);
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Fetching 5 recent matches for UserId=2, Status=Any"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"Calling GetRecentMatchesAsync: UserId=2, Limit=5, AcceptedStatus=null"), Times.Once());
            _mockLoggerManager!.Verify(logger => logger.LogInfo($"0 recent user matches fetched for UserId=2"), Times.Once());
            _mockAuditTrailService!.Verify(service => service.LogAuditTrail($"Getting recent user match result for user with ID 2", user), Times.Once());
        }
    }
}