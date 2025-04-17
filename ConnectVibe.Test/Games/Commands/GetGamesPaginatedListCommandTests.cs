
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Commands.GetAllGamesPaginatedListCommand;
using Fliq.Contracts.Games;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class GetGamesPaginatedListCommandTests
    {
        private Mock<IGamesRepository>? _mockGamesRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetGamesPaginatedListCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new GetGamesPaginatedListCommandHandler(
                _mockGamesRepository.Object, 
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_NoGamesFound_ReturnsError()
        {
            // Arrange
            var command = new GetGamesPaginatedListCommand(
                Page: 1,
                PageSize: 10,
                DatePlayedFrom: null,
                DatePlayedTo: null,
                Status: GameStatus.Done
            );

            _mockGamesRepository?.Setup(repo => repo.GetAllGamesListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>()))
                .ReturnsAsync((new List<GamesListItem>(), 0)); // Empty list
            _mockLogger?.Setup(l => l.LogInfo(It.IsAny<string>())).Verifiable();
            _mockLogger?.Setup(l => l.LogError(It.IsAny<string>())).Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError, "Expected an error when no games are found");
            Assert.AreEqual(Errors.Games.NoGamesFound, result.FirstError, "Expected NoGamesFound error");

            _mockLogger?.Verify(l => l.LogInfo("Fetching all games..."), Times.Once());
            _mockLogger?.Verify(l => l.LogError("No games found matching the provided filters"), Times.Once());
            //_mockLogger?.Verify(l => l.LogInfo(It.Is<string>(msg => msg.Contains("Returning")), Times.Never());
        }

        [TestMethod]
        public async Task Handle_GamesFound_ReturnsGamesListResponse()
        {
            // Arrange
            var command = new GetGamesPaginatedListCommand(
                Page: 1,
                PageSize: 10,
                DatePlayedFrom: null,
                DatePlayedTo: null,
                Status: GameStatus.Done
            );

            var gamesList = new List<GamesListItem>
            {
                new GamesListItem
                {
                    GameTitle = "Chess Match",
                    Players = "John Doe Vs Jane Smith",
                    Status = GameStatus.Done,
                    Stake = "$50",
                    Winner = "Jane Smith",
                    DatePlayed = new DateTime(2025, 1, 1)
                },
                new GamesListItem
                {
                    GameTitle = "Poker Game",
                    Players = "Alice Brown Vs Bob White",
                    Status = GameStatus.Done,
                    Stake = "$100",
                    Winner = "Bob White",
                    DatePlayed = new DateTime(2025, 1, 2)
                }
            };
            var totalCount = 5; // Simulating more games exist beyond this page

            _mockGamesRepository?
                .Setup(repo => repo.GetAllGamesListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>()))
                .ReturnsAsync((gamesList, totalCount));

            _mockLogger?.Setup(l => l.LogInfo(It.IsAny<string>())).Verifiable();
            _mockLogger?.Setup(l => l.LogError(It.IsAny<string>())).Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError, "Expected no error when games are found");
            Assert.IsNotNull(result.Value, "Expected a valid response");

            var response = result.Value;
            Assert.AreEqual(gamesList.Count, response.List.Count, "Expected the same number of games");
            Assert.AreEqual(totalCount, response.TotalCount, "Expected the correct total count");
            Assert.AreEqual(command.Page, response.Page, "Expected the correct page number");
            Assert.AreEqual(command.PageSize, response.PageSize, "Expected the correct page size");

            var firstGame = response.List[0];
            Assert.AreEqual("Chess Match", firstGame.GameTitle);
            Assert.AreEqual("John Doe Vs Jane Smith", firstGame.Players);
            Assert.AreEqual(GameStatus.Done, firstGame.Status);
            Assert.AreEqual("$50", firstGame.Stake);
            Assert.AreEqual("Jane Smith", firstGame.Winner);
            Assert.AreEqual(new DateTime(2025, 1, 1), firstGame.DatePlayed);

            _mockLogger?.Verify(l => l.LogInfo("Fetching all games..."), Times.Once());
            _mockLogger?.Verify(l => l.LogInfo($"Returning {gamesList.Count} games out of {totalCount} total matching filters"), Times.Once());
            _mockLogger?.Verify(l => l.LogError(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public async Task Handle_RepositoryCalledWithCorrectParameters()
        {
            // Arrange
            var datePlayedFrom = new DateTime(2025, 1, 1);
            var datePlayedTo = new DateTime(2025, 1, 31);
            var command = new GetGamesPaginatedListCommand(
                Page: 2,
                PageSize: 5,
                DatePlayedFrom: datePlayedFrom,
                DatePlayedTo: datePlayedTo,
                Status: GameStatus.InProgress
            );

            _mockGamesRepository?
                .Setup(repo => repo.GetAllGamesListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>()))
                .ReturnsAsync((new List<GamesListItem>(), 0));

            _mockLogger?.Setup(l => l.LogInfo(It.IsAny<string>())).Verifiable();
            _mockLogger?.Setup(l => l.LogError(It.IsAny<string>())).Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockGamesRepository?.Verify(repo =>
                repo.GetAllGamesListAsync(command.Page, command.PageSize, command.DatePlayedFrom, command.DatePlayedTo, (int?)command.Status), Times.Once(),
                "Repository should be called with the correct parameters");
        }
    }
}
