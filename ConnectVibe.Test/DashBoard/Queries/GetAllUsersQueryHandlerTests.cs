using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Queries.GetAllUser;
using Fliq.Domain.Entities;
using MapsterMapper;
using Moq;

[TestClass]
public class GetAllUsersQueryHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ILoggerManager> _loggerMock;
    private GetAllUsersQueryHandler _handler;

    [TestInitialize]
    public void Setup()
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
    public async Task Handle_ReturnsPagedUsers_WhenUsersExist()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 2;
        var hasSubscription = (bool?)null; // No filter
        var activeSince = (DateTime?)null; // No filter
        var roleName = (string)null;           // No filter
        var query = new GetAllUsersQuery(pageNumber, pageSize, hasSubscription, activeSince, roleName);

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                DisplayName = "User1",
                Email = "user1@example.com",
                DateCreated = DateTime.UtcNow.AddDays(-10),
                LastActiveAt = DateTime.UtcNow.AddDays(-1),
                Subscriptions = new List<Subscription>
                {
                    new Subscription { ProductId = "Premium", StartDate = DateTime.UtcNow.AddDays(-5) }
                }
               
            },
            new User
            {
                Id = 2,
                DisplayName = "User2",
                Email = "user2@example.com",
                DateCreated = DateTime.UtcNow.AddDays(-8),
                LastActiveAt = DateTime.UtcNow.AddDays(-2),
                Subscriptions = null
               
            }
        };

        _userRepositoryMock.Setup(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName))
            .Returns(users);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(2, result.Value.Count);

        var firstUser = result.Value[0];
        Assert.AreEqual("User1", firstUser.DisplayName);
        Assert.AreEqual("user1@example.com", firstUser.Email);
        Assert.AreEqual("Premium", firstUser.SubscriptionType);

        var secondUser = result.Value[1];
        Assert.AreEqual("User2", secondUser.DisplayName);
        Assert.AreEqual("user2@example.com", secondUser.Email);
        Assert.AreEqual("None", secondUser.SubscriptionType);

        _userRepositoryMock.Verify(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Getting users for page {pageNumber} with page size {pageSize}"), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Got {users.Count} users for page {pageNumber}"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ReturnsFilteredUsers_WhenSubscriptionFilterApplied()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var hasSubscription = true; // Filter for subscribed users only
        var activeSince = (DateTime?)null;
        var roleName = (string)null;
        var query = new GetAllUsersQuery(pageNumber, pageSize, hasSubscription, activeSince, roleName);

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                DisplayName = "User1",
                Email = "user1@example.com",
                DateCreated = DateTime.UtcNow.AddDays(-10),
                LastActiveAt = DateTime.UtcNow.AddDays(-1),
                Subscriptions = new List<Subscription>
                {
                    new Subscription { ProductId = "Premium", StartDate = DateTime.UtcNow.AddDays(-5) }
                }
            }
        };

        _userRepositoryMock.Setup(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName))
            .Returns(users);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(1, result.Value.Count);
        Assert.AreEqual("User1", result.Value[0].DisplayName);
        Assert.AreEqual("Premium", result.Value[0].SubscriptionType);

        _userRepositoryMock.Verify(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Getting users for page {pageNumber} with page size {pageSize}"), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Got {users.Count} users for page {pageNumber}"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ReturnsFilteredUsers_WhenActiveSinceFilterApplied()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var hasSubscription = (bool?)null;
        var activeSince = DateTime.UtcNow.AddDays(-5); // Filter for users active in the last 5 days
        var roleName = (string)null;
        var query = new GetAllUsersQuery(pageNumber, pageSize, hasSubscription, activeSince, roleName);

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                DisplayName = "User1",
                Email = "user1@example.com",
                DateCreated = DateTime.UtcNow.AddDays(-10),
                LastActiveAt = DateTime.UtcNow.AddDays(-1),
                Subscriptions = null
            }
        };

        _userRepositoryMock.Setup(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName))
            .Returns(users);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(1, result.Value.Count);
        Assert.AreEqual("User1", result.Value[0].DisplayName);
        

        _userRepositoryMock.Verify(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Getting users for page {pageNumber} with page size {pageSize}"), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Got {users.Count} users for page {pageNumber}"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ReturnsFilteredUsers_WhenRoleFilterApplied()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var hasSubscription = (bool?)null;
        var activeSince = (DateTime?)null;
        var roleName = "Admin"; // Filter for admins only
        var query = new GetAllUsersQuery(pageNumber, pageSize, hasSubscription, activeSince, roleName);

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                DisplayName = "Admin1",
                Email = "admin1@example.com",
                DateCreated = DateTime.UtcNow.AddDays(-10),
                LastActiveAt = DateTime.UtcNow.AddDays(-1),
                Subscriptions = null,
               
            }
        };

        _userRepositoryMock.Setup(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName))
            .Returns(users);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(1, result.Value.Count);
        Assert.AreEqual("Admin1", result.Value[0].DisplayName);
        

        _userRepositoryMock.Verify(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Getting users for page {pageNumber} with page size {pageSize}"), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Got {users.Count} users for page {pageNumber}"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ReturnsEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var hasSubscription = (bool?)null;
        var activeSince = (DateTime?)null;
        var roleName = (string)null;
        var query = new GetAllUsersQuery(pageNumber, pageSize, hasSubscription, activeSince, roleName);

        var emptyUsers = new List<User>();
        _userRepositoryMock.Setup(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName))
            .Returns(emptyUsers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(0, result.Value.Count);

        _userRepositoryMock.Verify(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Getting users for page {pageNumber} with page size {pageSize}"), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Got 0 users for page {pageNumber}"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ReturnsLatestSubscription_WhenMultipleSubscriptionsExist()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 1;
        var hasSubscription = true; // Filter for subscribed users
        var activeSince = (DateTime?)null;
        var roleName = (string)null;
        var query = new GetAllUsersQuery(pageNumber, pageSize, hasSubscription, activeSince, roleName);

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                DisplayName = "User1",
                Email = "user1@example.com",
                DateCreated = DateTime.UtcNow.AddDays(-20),
                LastActiveAt = DateTime.UtcNow.AddDays(-1),
                Subscriptions = new List<Subscription>
                {
                    new Subscription { ProductId = "Basic", StartDate = DateTime.UtcNow.AddDays(-10) },
                    new Subscription { ProductId = "Premium", StartDate = DateTime.UtcNow.AddDays(-5) },
                    new Subscription { ProductId = "Standard", StartDate = DateTime.UtcNow.AddDays(-15) }
                }
            }
        };

        _userRepositoryMock.Setup(r => r.GetAllUsersForDashBoard(pageNumber, pageSize, hasSubscription, activeSince, roleName))
            .Returns(users);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(1, result.Value.Count);
        Assert.AreEqual("Premium", result.Value[0].SubscriptionType); // Should return the most recent subscription
    }
}