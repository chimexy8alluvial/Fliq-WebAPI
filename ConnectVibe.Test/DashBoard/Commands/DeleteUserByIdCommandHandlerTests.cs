using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Command.DeleteUser;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using MapsterMapper;
using Moq;

[TestClass]
public class DeleteUserByIdCommandHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ILoggerManager> _loggerMock;
    private DeleteUserByIdCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerManager>();
        _handler = new DeleteUserByIdCommandHandler(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidUserId_DeletesUserAndReturnsSuccess()
    {
        // Arrange
        int userId = 1;
        var command = new DeleteUserByIdCommand(userId);
        var user = new User { Id = userId, DisplayName = "TestUser", IsDeleted = false };

        _userRepositoryMock.Setup(r => r.GetUserById(userId)).Returns(user);
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual($"{user.Id} has been deleted", result.Value.Message);

        Assert.IsTrue(user.IsDeleted); // Verify the user was marked as deleted
        _userRepositoryMock.Verify(r => r.Update(user), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"Deleting user with {userId} "), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"User with Id {userId} was deleted"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
    {
        // Arrange
        int userId = 1;
        var command = new DeleteUserByIdCommand(userId);

        _userRepositoryMock.Setup(r => r.GetUserById(userId)).Returns((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        _loggerMock.Verify(l => l.LogError("User not found"), Times.Once());
        _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
    }

    [TestMethod]
    public async Task Handle_AlreadyDeletedUser_ReturnsUserAlreadyDeletedError()
    {
        // Arrange
        int userId = 1;
        var command = new DeleteUserByIdCommand(userId);
        var user = new User { Id = userId, DisplayName = "TestUser", IsDeleted = true };

        _userRepositoryMock.Setup(r => r.GetUserById(userId)).Returns(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.User.UserAlreadyDeleted, result.FirstError);
        _loggerMock.Verify(l => l.LogError($"This user with id {user.Id} has been deleted before"), Times.Once());
        _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
    }

    [TestMethod]
    public async Task Handle_LogsInformation_AtStartAndEnd()
    {
        // Arrange
        int userId = 1;
        var command = new DeleteUserByIdCommand(userId);
        var user = new User { Id = userId, DisplayName = "TestUser", IsDeleted = false };

        _userRepositoryMock.Setup(r => r.GetUserById(userId)).Returns(user);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.LogInfo($"Deleting user with {userId} "), Times.Once());
        _loggerMock.Verify(l => l.LogInfo($"User with Id {userId} was deleted"), Times.Once());
    }
}