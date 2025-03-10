

using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.DatingEnvironment.Commands.BlindDates;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.DatingEnvironment;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Fliq.Test.Dating.Commands
{
    [TestClass]
    public class JoinBlindDateCommandHandlerTests
    {
        private Mock<IBlindDateRepository> _blindDateRepositoryMock;
        private Mock<IBlindDateParticipantRepository> _blindDateParticipantRepositoryMock;
        private Mock<ILoggerManager> _loggerMock;
        private Mock<IHubContext<BlindDateHub>> _hubContextMock;
        private Mock<IClientProxy> _clientProxyMock;
        private JoinBlindDateCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _blindDateRepositoryMock = new Mock<IBlindDateRepository>();
            _blindDateParticipantRepositoryMock = new Mock<IBlindDateParticipantRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _hubContextMock = new Mock<IHubContext<BlindDateHub>>();
            _clientProxyMock = new Mock<IClientProxy>();

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            _handler = new JoinBlindDateCommandHandler(
                _blindDateRepositoryMock.Object,
                _blindDateParticipantRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenBlindDateDoesNotExist()
        {
            // Arrange
            var command = new JoinBlindDateCommand(1, 10);
            _blindDateRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BlindDateId))
                .ReturnsAsync((BlindDate)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.BlindDateNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenSessionHasEnded()
        {
            // Arrange
            var blindDate = new BlindDate { Id = 10, SessionEndTime = DateTime.UtcNow.AddMinutes(-5) };
            var command = new JoinBlindDateCommand(1, 10);
            _blindDateRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BlindDateId))
                .ReturnsAsync(blindDate);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.BlindDateSessionEnded, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenUserAlreadyJoined()
        {
            // Arrange
            var blindDate = new BlindDate { Id = 10 };
            var participant = new BlindDateParticipant { UserId = 1, BlindDateId = 10 };
            var command = new JoinBlindDateCommand(1, 10);

            _blindDateRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BlindDateId))
                .ReturnsAsync(blindDate);
            _blindDateParticipantRepositoryMock.Setup(repo => repo.GetByUserAndBlindDateId(command.UserId, command.BlindDateId))
                .ReturnsAsync(participant);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.AlreadyJoined, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnError_WhenBlindDateIsFull()
        {
            // Arrange
            var blindDate = new BlindDate { Id = 10, NumberOfParticipants = 2 };
            var command = new JoinBlindDateCommand(1, 10);

            _blindDateRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BlindDateId))
                .ReturnsAsync(blindDate);
            _blindDateParticipantRepositoryMock.Setup(repo => repo.CountByBlindDateId(command.BlindDateId))
                .ReturnsAsync(2);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.BlindDateFull, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ShouldJoinBlindDateSuccessfully()
        {
            // Arrange
            var blindDate = new BlindDate { Id = 10, NumberOfParticipants = 5 };
            var command = new JoinBlindDateCommand(1, 10);

            _blindDateRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BlindDateId))
                .ReturnsAsync(blindDate);
            _blindDateParticipantRepositoryMock.Setup(repo => repo.GetByUserAndBlindDateId(command.UserId, command.BlindDateId))
                .ReturnsAsync((BlindDateParticipant)null);
            _blindDateParticipantRepositoryMock.Setup(repo => repo.CountByBlindDateId(command.BlindDateId))
                .ReturnsAsync(1);
            _blindDateParticipantRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<BlindDateParticipant>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            //Assert.IsTrue(result.IsSuccess);
            _blindDateParticipantRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<BlindDateParticipant>()), Times.Once);
            _clientProxyMock.Verify(client => client.SendCoreAsync("UserJoined", It.IsAny<object[]>(), default), Times.Once);
        }
    }
}
