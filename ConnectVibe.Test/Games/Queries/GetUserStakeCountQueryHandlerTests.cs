

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Queries.StakeCount;
using Moq;

namespace Fliq.Test.Games.Queries
{
    [TestClass]
    public class GetUserStakeCountQueryHandlerTests
    {
        private Mock<IGamesRepository> _gamesRepositoryMock;
        private Mock<ILoggerManager> _loggerMock;
        private GetUserStakeCountQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _gamesRepositoryMock = new Mock<IGamesRepository>();
            _loggerMock = new Mock<ILoggerManager>();

            _handler = new GetUserStakeCountQueryHandler(_gamesRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnHalfOfTotalStakeCount()
        {
            // Arrange
            int userId = 1;
            int totalStakes = 10; // Suppose the stored procedure returns 10
            int expectedStakeCount = totalStakes / 2;

            var query = new GetUserStakeCountQuery(userId);

            _gamesRepositoryMock
                .Setup(repo => repo.GetStakeCountByUserId(userId))
                .ReturnsAsync(totalStakes);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedStakeCount, result.Value.Count);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnZero_WhenUserHasNoStakes()
        {
            // Arrange
            int userId = 2;
            int totalStakes = 0;

            var query = new GetUserStakeCountQuery(userId);

            _gamesRepositoryMock
                .Setup(repo => repo.GetStakeCountByUserId(userId))
                .ReturnsAsync(totalStakes);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Value.Count);
        }
    }
}
