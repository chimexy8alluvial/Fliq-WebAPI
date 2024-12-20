using Moq;
using Fliq.Application.Games.Commands.SubmitAnswer;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Enums;
using Fliq.Domain.Common.Errors;
using Fliq.Application.Common.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Test.Games.Commands
{
    [TestClass]
    public class SubmitAnswerCommandHandlerTests
    {
        private Mock<IGamesRepository> _mockGamesRepository;
        private Mock<ILoggerManager> _mockLogger;
        private SubmitAnswerCommandHandler _handler;
        private Mock<IHubContext<GameHub>> _mockHub;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<IGamesRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _mockHub = new Mock<IHubContext<GameHub>>();
            _handler = new SubmitAnswerCommandHandler(_mockGamesRepository.Object, _mockLogger.Object, _mockHub.Object);
        }

        [TestMethod]
        public async Task Handle_ValidAnswer_UpdatesScoresAndAdvancesTurn()
        {
            // Arrange
            var session = new GameSession
            {
                Id = 1,
                Status = GameStatus.InProgress,
                GameId = 100,
                Player1Id = 1,
                Player2Id = 2,
                Player1Score = 0,
                Player2Score = 0,
            };

            var question = new GameQuestion
            {
                QuestionText = "What is 2+2?",
                CorrectAnswer = "4"
            };

            var questions = new List<GameQuestion> { question };

            _mockGamesRepository.Setup(repo => repo.GetGameSessionById(session.Id)).Returns(session);
            _mockGamesRepository.Setup(repo => repo.GetQuestionsByGameId(session.GameId, It.IsAny<int>(), It.IsAny<int>()))
                .Returns(questions);

            var command = new SubmitAnswerCommand(SessionId: 1, Player1Score: 2, Player2Score: 2);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1, session.Player2Score);
            Assert.AreEqual(0, session.Player1Score);

            _mockGamesRepository.Verify(repo => repo.UpdateGameSession(It.IsAny<GameSession>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_SessionNotFound_ReturnsGameNotFoundError()
        {
            // Arrange
            _mockGamesRepository.Setup(repo => repo.GetGameSessionById(It.IsAny<int>())).Returns((GameSession)null);

            var command = new SubmitAnswerCommand(SessionId: 999, Player1Score: 2, Player2Score: 1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Games.GameNotFound.Code, result.FirstError.Code);

            _mockLogger.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Session 999 not found"))), Times.Once);
            _mockGamesRepository.Verify(repo => repo.UpdateGameSession(It.IsAny<GameSession>()), Times.Never);
        }
    }
}