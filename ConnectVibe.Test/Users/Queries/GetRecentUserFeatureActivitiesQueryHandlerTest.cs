using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Users.Common;
using Fliq.Application.Users.Queries;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.UserFeatureActivities;
using MapsterMapper;
using Moq;

namespace Fliq.Test.Users.Queries
{
    [TestClass]
    public class GetRecentUserFeatureActivitiesQueryHandlerTest
    {
        private Mock<IUserFeatureActivityRepository>? _mockUserFeatureActivityRepository;
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private Mock<IMapper>? _mockMapper;
        private Mock<IAuditTrailService>? _mockAuditTrailService;
        private GetRecentUserFeatureActivitiesQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockUserFeatureActivityRepository = new Mock<IUserFeatureActivityRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();
            _mockMapper = new Mock<IMapper>();
            _mockAuditTrailService = new Mock<IAuditTrailService>();

            _handler = new GetRecentUserFeatureActivitiesQueryHandler(
                _mockUserFeatureActivityRepository.Object,
                _mockUserRepository.Object,
                _mockLoggerManager.Object,
                _mockMapper.Object,
                _mockAuditTrailService.Object
            );
        }

        [TestMethod]
        public async Task Handle_AdminUserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var query = new GetRecentUserFeatureActivitiesQuery(1, 2, 5);
            _mockUserRepository.Setup(repo => repo.GetUserById(1)).Returns((User)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }


        [TestMethod]
        public async Task Handle_FeatureUserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            _mockUserRepository.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository.Setup(repo => repo.GetUserById(2)).Returns((User)null);
            var query = new GetRecentUserFeatureActivitiesQuery(1, 2, 5);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_ReturnsRecentFeatureActivities()
        {
            // Arrange
            var adminUser = new User { Id = 1, RoleId = 1 };
            var featureUser = new User { Id = 2, RoleId = 3 };
            var query = new GetRecentUserFeatureActivitiesQuery(1, 2, 5);

            var activities = new List<UserFeatureActivity>
            {
                new UserFeatureActivity { UserId = 2, Feature = "Login", LastActiveAt = DateTime.UtcNow },
                new UserFeatureActivity { UserId = 2, Feature = "Update Profile", LastActiveAt = DateTime.UtcNow.AddMinutes(-5) }
            };

            var expectedMappedResult = new List<GetRecentUserFeatureActivityResult>
            {
                new GetRecentUserFeatureActivityResult(2, "Login", DateTime.UtcNow),
                new GetRecentUserFeatureActivityResult(2, "Update Profile", DateTime.UtcNow.AddMinutes(-5))
            };

            _mockUserRepository.Setup(repo => repo.GetUserById(1)).Returns(adminUser);
            _mockUserRepository.Setup(repo => repo.GetUserById(2)).Returns(featureUser);
            _mockUserFeatureActivityRepository
                .Setup(repo => repo.GetRecentUserFeatureActivitiesAsync(2, 5))
                .ReturnsAsync(activities);
            _mockMapper
                .Setup(m => m.Map<List<GetRecentUserFeatureActivityResult>>(activities))
                .Returns(expectedMappedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            CollectionAssert.AreEqual(expectedMappedResult, result.Value);
        }
    }
}
