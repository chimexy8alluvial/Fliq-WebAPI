using Fliq.Application.Common.Interfaces.Persistence;
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
        private Mock<IMatchProfileRepository> _mockMatchProfileRepository;
        private Mock<IMediator> _mockMediator;
        private RejectMatchRequestComandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMatchProfileRepository = new Mock<IMatchProfileRepository>();
            _mockMediator = new Mock<IMediator>();

            _handler = new RejectMatchRequestComandHandler(
                _mockMatchProfileRepository.Object,
                _mockMediator.Object
            );
        }

        [TestMethod]
        public async Task Handle_MatchProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var command = new RejectMatchRequestCommand
            {
                Id = 1,
                UserId = 10
            };

            _mockMatchProfileRepository
                .Setup(repo => repo.GetMatchProfileById(command.Id))
                .Returns((MatchRequest)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Profile.ProfileNotFound));
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

            _mockMatchProfileRepository
                .Setup(repo => repo.GetMatchProfileById(command.Id))
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

            _mockMatchProfileRepository
                .Setup(repo => repo.GetMatchProfileById(command.Id))
                .Returns(matchProfile);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMediator.Verify(mediator => mediator.Publish(
                It.Is<MatchRejectedEvent>(e =>
                    e.UserId == command.UserId),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}