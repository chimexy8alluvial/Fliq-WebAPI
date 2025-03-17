
using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands.BlindDates;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Fliq.Test.Dating.Commands
{
    [TestClass]
    public class StartBlindDateCommandHandlerTest
    {
        private Mock<IBlindDateRepository> _mockBlindDateRepository;
        private Mock<IBlindDateParticipantRepository> _mockBlindDateParticipantRepository;
        private Mock<ILoggerManager> _mockLoggerManager;
        private StartBlindDateCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockBlindDateRepository = new Mock<IBlindDateRepository>();
            _mockBlindDateParticipantRepository = new Mock<IBlindDateParticipantRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();

            _handler = new StartBlindDateCommandHandler(
                _mockBlindDateRepository.Object,
                _mockBlindDateParticipantRepository.Object,
                _mockLoggerManager.Object
            );
        }

        [TestMethod]
        public async Task Handle_BlindDateNotFound_ReturnsError()
        {
            // Arrange
            var command = new StartBlindDateCommand(1, 100);
            _mockBlindDateRepository.Setup(repo => repo.GetByIdAsync(command.BlindDateId))
                .ReturnsAsync((BlindDate)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Dating.BlindDateNotFound));
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("not found"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_UserNotCreator_ReturnsError()
        {
            // Arrange
            var command = new StartBlindDateCommand(1, 100);
            var blindDate = new BlindDate { Id = 100, Status = DateStatus.Pending };
            var creator = new BlindDateParticipant { UserId = 2 }; // Different user

            _mockBlindDateRepository.Setup(repo => repo.GetByIdAsync(command.BlindDateId)).ReturnsAsync(blindDate);
            _mockBlindDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(command.BlindDateId)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Dating.NotSessionCreator));
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("not the creator"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_BlindDateAlreadyStarted_ReturnsError()
        {
            // Arrange
            var command = new StartBlindDateCommand(1, 100);
            var blindDate = new BlindDate { Id = 100, Status = DateStatus.Ongoing, SessionStartTime = DateTime.UtcNow };
            var creator = new BlindDateParticipant { UserId = 1 };

            _mockBlindDateRepository.Setup(repo => repo.GetByIdAsync(command.BlindDateId)).ReturnsAsync(blindDate);
            _mockBlindDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(command.BlindDateId)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Dating.BlindDateAlreadyStarted));
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("already started"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_BlindDateAlreadyEnded_ReturnsError()
        {
            // Arrange
            var command = new StartBlindDateCommand(1, 100);
            var blindDate = new BlindDate { Id = 100, Status = DateStatus.Completed, SessionStartTime = DateTime.UtcNow };
            var creator = new BlindDateParticipant { UserId = 1 };

            _mockBlindDateRepository.Setup(repo => repo.GetByIdAsync(command.BlindDateId)).ReturnsAsync(blindDate);
            _mockBlindDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(command.BlindDateId)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Dating.BlindDateAlreadyEnded));
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("already ended"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_BlindDateCancelled_ReturnsError()
        {
            // Arrange
            var command = new StartBlindDateCommand(1, 100);
            var blindDate = new BlindDate { Id = 100, Status = DateStatus.Cancelled };
            var creator = new BlindDateParticipant { UserId = 1 };

            _mockBlindDateRepository.Setup(repo => repo.GetByIdAsync(command.BlindDateId)).ReturnsAsync(blindDate);
            _mockBlindDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(command.BlindDateId)).ReturnsAsync(creator);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Dating.BlindDateCancelled));
            _mockLoggerManager.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("cancelled"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ValidRequest_StartsBlindDateSuccessfully()
        {
            // Arrange
            var command = new StartBlindDateCommand(1, 100);
            var blindDate = new BlindDate { Id = 100, Status = DateStatus.Pending };
            var creator = new BlindDateParticipant { UserId = 1 };

            _mockBlindDateRepository.Setup(repo => repo.GetByIdAsync(command.BlindDateId)).ReturnsAsync(blindDate);
            _mockBlindDateParticipantRepository.Setup(repo => repo.GetCreatorByBlindDateId(command.BlindDateId)).ReturnsAsync(creator);
            _mockBlindDateRepository.Setup(repo => repo.UpdateAsync(It.IsAny<BlindDate>())).Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value.SessionStartTime <= DateTime.UtcNow);

            _mockBlindDateRepository.Verify(repo => repo.UpdateAsync(It.IsAny<BlindDate>()), Times.Once);
            _mockLoggerManager.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("started successfully"))), Times.Once);
        }
    }
}
