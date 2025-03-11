using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.GetAllUser;
using Fliq.Domain.Entities;
using MapsterMapper;
using Moq;

namespace Fliq.Application.Tests.DashBoard.Queries
{
    [TestClass]
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerManager> _loggerMock;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllUsersQueryHandler(
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsPaginatedUsers()
        {
            // Arrange
            var query = new GetAllUsersQuery(1, 2); // Page 1, 2 users per page

            var users = new List<User>
            {
                new User
                {
                    DisplayName = "JohnDoe",
                    Email = "john@example.com",
                    DateCreated = new DateTime(2023, 1, 1),
                    LastActiveAt = new DateTime(2025, 3, 11),
                    Subscriptions = new List<Subscription>
                    {
                        new Subscription { ProductId = "Premium", StartDate = new DateTime(2024, 1, 1) }
                    }
                },
                new User
                {
                    DisplayName = "JaneDoe",
                    Email = "jane@example.com",
                    DateCreated = new DateTime(2023, 2, 1),
                    LastActiveAt = new DateTime(2025, 3, 10),
                    Subscriptions = null
                }
            };

            _userRepositoryMock
                .Setup(repo => repo.GetAllUsersForDashBoard(query.PageNumber, query.PageSize))
                .Returns(users);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<CreateUserResult>));
            Assert.AreEqual(2, result.Count);

            var firstUser = result[0];
            Assert.AreEqual("JohnDoe", firstUser.DisplayName);
            Assert.AreEqual("john@example.com", firstUser.Email);
            Assert.AreEqual("Premium", firstUser.SubscriptionType);
            Assert.AreEqual(new DateTime(2023, 1, 1), firstUser.DateJoined);
            Assert.AreEqual(new DateTime(2025, 3, 11), firstUser.LastOnline);

            var secondUser = result[1];
            Assert.AreEqual("JaneDoe", secondUser.DisplayName);
            Assert.AreEqual("jane@example.com", secondUser.Email);
            Assert.AreEqual("None", secondUser.SubscriptionType);
            Assert.AreEqual(new DateTime(2023, 2, 1), secondUser.DateJoined);
            Assert.AreEqual(new DateTime(2025, 3, 10), secondUser.LastOnline);

            _loggerMock.Verify(logger => logger.LogInfo("Getting users for page 1 with page size 2"), Times.Once());
            _loggerMock.Verify(logger => logger.LogInfo("Got 2 users for page 1"), Times.Once());
        }

        

        [TestMethod]
        public async Task Handle_EmptyUserList_ReturnsEmptyResult()
        {
            // Arrange
            var query = new GetAllUsersQuery(1, 10);
            _userRepositoryMock
                .Setup(repo => repo.GetAllUsersForDashBoard(query.PageNumber, query.PageSize))
                .Returns(new List<User>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _loggerMock.Verify(logger => logger.LogInfo("Got 0 users for page 1"), Times.Once());
        }
    }
}