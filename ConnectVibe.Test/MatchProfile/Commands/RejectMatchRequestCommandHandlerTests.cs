using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Commands.RejectMatch;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Enums;
using MediatR;
using Moq;

namespace Fliq.Test.MatchProfile.Commands
{
    [TestClass]
    public class RejectMatchRequestCommandHandlerTests
    {
        private Mock<IMatchProfileRepository>? _mockMatchProfileRepository;
        private Mock<IMediator>? _mockMediator;
        private Mock<ILoggerManager>? _mockLogger;
        private RejectMatchRequestComandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMatchProfileRepository = new Mock<IMatchProfileRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new RejectMatchRequestComandHandler(
                _mockMatchProfileRepository.Object,
                _mockMediator.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_MatchRequestNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var command = new RejectMatchRequestCommand
            {
                Id = 1,
                UserId = 10
            };

            _mockMatchProfileRepository?
                .Setup(repo => repo.GetMatchRequestById(command.Id))
                .Returns((MatchRequest?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.MatchRequest.RequestNotFound));
        }

        [TestMethod]
        public async Task Handle_SuccessfulMatchRejection_ReturnsRejectMatchResult()
        {
            // Arrange
            var command = new RejectMatchRequestCommand
            {
                Id = 1,
                UserId = 10
            };

            var matchProfile = new MatchRequest
            {
                Id = 1,
                MatchInitiatorUserId = 2,
                MatchReceiverUserId = 10,
                MatchRequestStatus = MatchRequestStatus.Pending
            };

            _mockMatchProfileRepository?
                .Setup(repo => repo.GetMatchRequestById(command.Id))
                .Returns(matchProfile);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(MatchRequestStatus.Rejected, result.Value.matchRequestStatus);
            Assert.AreEqual(matchProfile.MatchInitiatorUserId, result.Value.MatchInitiatorUserId);
        }

        [TestMethod]
        public async Task Handle_NotificationSent_VerifiesEventPublished()
        {
            // Arrange
            var command = new RejectMatchRequestCommand
            {
                Id = 1,
                UserId = 10
            };

            var matchProfile = new MatchRequest
            {
                Id = 1,
                MatchInitiatorUserId = 2,
                MatchReceiverUserId = 10,
                MatchRequestStatus = MatchRequestStatus.Pending
            };

            _mockMatchProfileRepository?
                .Setup(repo => repo.GetMatchRequestById(command.Id))
                .Returns(matchProfile);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMediator?.Verify(mediator => mediator.Publish(
                It.Is<MatchRejectedEvent>(e =>
                    e.UserId == command.UserId),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}