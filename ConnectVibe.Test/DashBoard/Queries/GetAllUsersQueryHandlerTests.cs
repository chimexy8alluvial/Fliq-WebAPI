using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.GetAllUser;
using Moq;

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
        _handler = new GetAllUsersQueryHandler(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var pagination = new PaginationRequest(1, 10);
        var query = new GetAllUsersQuery(pagination);
        var expectedUsers = new List<GetUsersResult>
        {
            new GetUsersResult("User1", "user1@example.com", "Free", DateTime.UtcNow, DateTime.UtcNow),
            new GetUsersResult("User2", "user2@example.com", "Premium", DateTime.UtcNow, DateTime.UtcNow)
        };

        _userRepositoryMock!
            .Setup(x => x.GetAllUsersForDashBoardAsync(It.IsAny<GetUsersListRequest>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(expectedUsers.Count, result.Value.Count);
        Assert.AreEqual(expectedUsers[0].Email, result.Value[0].Email);
        _loggerMock!.Verify(x => x.LogInfo(It.IsAny<string>()), Times.Exactly(2));
    }

   

    [TestMethod]
    public async Task Handle_NullUsers_ReturnsFailureError()
    {
        // Arrange
        var pagination = new PaginationRequest(1, 10);
        var query = new GetAllUsersQuery(pagination);

        _userRepositoryMock!
            .Setup(x => x.GetAllUsersForDashBoardAsync(It.IsAny<GetUsersListRequest>()))
            .ReturnsAsync((IEnumerable<GetUsersResult>)null!);

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(ErrorType.Failure, result.FirstError.Type);
        Assert.AreEqual("DatabaseError", result.FirstError.Code);
    }

    [TestMethod]
    public async Task Handle_RepositoryThrowsException_ReturnsFailureError()
    {
        // Arrange
        var pagination = new PaginationRequest(1, 10);
        var query = new GetAllUsersQuery(pagination);

        _userRepositoryMock!
            .Setup(x => x.GetAllUsersForDashBoardAsync(It.IsAny<GetUsersListRequest>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _handler!.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(ErrorType.Failure, result.FirstError.Type);
        Assert.AreEqual("UnexpectedError", result.FirstError.Code);
        _loggerMock!.Verify(x => x.LogError(It.IsAny<string>()), Times.Once());
    }
}