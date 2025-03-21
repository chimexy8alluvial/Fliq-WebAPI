using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.GetAllUser;
using Fliq.Domain.Entities;
using Moq;

namespace Fliq.Application.Tests.DashBoard.Queries
{
    [TestClass]
    public class GetAllUsersQueryHandlerTests
    {
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetAllUsersQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllUsersQueryHandler(
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_ReturnsUserList()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllUsersQuery(pagination);

            var users = new List<User>
            {
                new ()
                {
                    DisplayName = "John Doe",
                    Email = "john@example.com",
                    DateCreated = DateTime.UtcNow.AddDays(-30),
                    LastActiveAt = DateTime.UtcNow.AddDays(-1),
                    Subscriptions =
                    [
                        new Subscription { ProductId = "Premium", StartDate = DateTime.UtcNow }
                    ]
                }
            };

            _userRepositoryMock?
                .Setup(r => r.GetAllUsersForDashBoard(It.IsAny<GetUsersListRequest>()))
                .Returns(users);

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual("John Doe", result.Value[0].DisplayName);
            Assert.AreEqual("john@example.com", result.Value[0].Email);
            Assert.AreEqual("Premium", result.Value[0].SubscriptionType);
            _loggerMock?.Verify(l => l.LogInfo(It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task Handle_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllUsersQuery(pagination);

            _userRepositoryMock?
                .Setup(r => r.GetAllUsersForDashBoard(It.IsAny<GetUsersListRequest>()))
                .Returns([]);

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Value.Count);
        }

        [TestMethod]
        public async Task Handle_WithFilters_PassesFiltersToRepository()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllUsersQuery(
                pagination,
                HasSubscription: true,
                ActiveSince: DateTime.UtcNow.AddDays(-30),
                RoleName: "Admin"
            );

            GetUsersListRequest capturedRequest = null!;
            _userRepositoryMock?
                .Setup(r => r.GetAllUsersForDashBoard(It.IsAny<GetUsersListRequest>()))
                .Callback<GetUsersListRequest>(req => capturedRequest = req)
                .Returns([]);

            // Act
            await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest.HasSubscription);
            Assert.AreEqual(query.ActiveSince, capturedRequest.ActiveSince);
            Assert.AreEqual("Admin", capturedRequest.RoleName);
            Assert.AreEqual(pagination, capturedRequest.PaginationRequest);
        }

        [TestMethod]
        public async Task Handle_UserWithNoSubscription_ReturnsFreeType()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllUsersQuery(pagination);

            var users = new List<User>
            {
                new ()
                {
                    DisplayName = "Jane Doe",
                    Email = "jane@example.com",
                    DateCreated = DateTime.UtcNow,
                    LastActiveAt = DateTime.UtcNow,
                    Subscriptions = null
                }
            };

            _userRepositoryMock?
                .Setup(r => r.GetAllUsersForDashBoard(It.IsAny<GetUsersListRequest>()))
                .Returns(users);

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual("Free", result.Value[0].SubscriptionType);
        }

        [TestMethod]
        public async Task Handle_MultipleSubscriptions_ReturnsLatest()
        {
            // Arrange
            var pagination = new PaginationRequest(1, 10);
            var query = new GetAllUsersQuery(pagination);

            var users = new List<User>
            {
                new ()
                {
                    DisplayName = "Test User",
                    Email = "test@example.com",
                    Subscriptions = new List<Subscription>
                    {
                        new() { ProductId = "Basic", StartDate = DateTime.UtcNow.AddDays(-10) },
                        new() { ProductId = "Premium", StartDate = DateTime.UtcNow }
                    }
                }
            };

            _userRepositoryMock?
                .Setup(r => r.GetAllUsersForDashBoard(It.IsAny<GetUsersListRequest>()))
                .Returns(users);

            // Act
            var result = await _handler?.Handle(query, CancellationToken.None)!;

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual("Premium", result.Value[0].SubscriptionType);
        }
    }

   
}