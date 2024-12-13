using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Commands.CreateGame;
using Fliq.Domain.Entities.Games;
using Moq;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class CreateGameCommandHandlerTests
    {
        private Mock<IGamesRepository> _mockGamesRepository;
        private Mock<ILoggerManager> _mockLogger;
        private CreateGameCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new CreateGameCommandHandler(
                _mockGamesRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_CreatesGameSuccessfully()
        {
            // Arrange
            var command = new CreateGameCommand(
                Name: "Trivia Game",
                Description: "A fun trivia game.",
                RequiresLevel: true,
                RequiresTheme: false,
                RequiresCategory: true
            );

            var game = new Game
            {
                Id = 1,
                Name = command.Name,
                Description = command.Description,
                RequiresLevel = command.RequiresLevel,
                RequiresTheme = command.RequiresTheme,
                RequiresCategory = command.RequiresCategory
            };

            _mockGamesRepository
                .Setup(repo => repo.AddGame(It.IsAny<Game>()))
                .Callback<Game>(g => g.Id = 1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(game.Id, result.Value.Game.Id);
            Assert.AreEqual(game.Name, result.Value.Game.Name);
            Assert.AreEqual(game.Description, result.Value.Game.Description);

            _mockGamesRepository.Verify(repo => repo.AddGame(It.IsAny<Game>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_LogsMessagesDuringExecution()
        {
            // Arrange
            var command = new CreateGameCommand(
                Name: "Chess Game",
                Description: "A strategy game.",
                RequiresLevel: false,
                RequiresTheme: true,
                RequiresCategory: false
            );

            _mockGamesRepository
                .Setup(repo => repo.AddGame(It.IsAny<Game>()))
                .Callback<Game>(g => g.Id = 2);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Creating game: Chess Game"))), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Game created: 2"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_AddGameIsCalledWithCorrectGameData()
        {
            // Arrange
            var command = new CreateGameCommand(
                Name: "Puzzle Game",
                Description: "A mind-challenging puzzle game.",
                RequiresLevel: true,
                RequiresTheme: true,
                RequiresCategory: false
            );

            Game capturedGame = null;

            _mockGamesRepository
                .Setup(repo => repo.AddGame(It.IsAny<Game>()))
                .Callback<Game>(g => capturedGame = g);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(capturedGame);
            Assert.AreEqual(command.Name, capturedGame.Name);
            Assert.AreEqual(command.Description, capturedGame.Description);
            Assert.AreEqual(command.RequiresLevel, capturedGame.RequiresLevel);
            Assert.AreEqual(command.RequiresTheme, capturedGame.RequiresTheme);
            Assert.AreEqual(command.RequiresCategory, capturedGame.RequiresCategory);
        }
    }
}