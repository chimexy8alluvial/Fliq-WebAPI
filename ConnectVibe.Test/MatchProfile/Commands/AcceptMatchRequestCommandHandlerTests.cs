using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.MatchedProfile.Commands.AcceptedMatch;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;
using Moq;

namespace Fliq.Test.MatchProfile.Commands
{
    [TestClass]
    public class AcceptMatchRequestCommandHandlerTests
    {
        private Mock<IMapper> _mockMapper;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IMatchProfileRepository> _mockMatchProfileRepository;
        private Mock<IMediator> _mockMediator;
        private AcceptMatchRequestCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMatchProfileRepository = new Mock<IMatchProfileRepository>();
            _mockMediator = new Mock<IMediator>();

            _handler = new AcceptMatchRequestCommandHandler(
                _mockMapper.Object,
                _mockUserRepository.Object,
                _mockMatchProfileRepository.Object,
                _mockMediator.Object
            );
        }

        [TestMethod]
        public async Task Handle_MatchProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var command = new AcceptMatchRequestCommand
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
        public async Task Handle_SuccessfulMatchRequest_ReturnsCreateAcceptMatchResult()
        {
            // Arrange
            var command = new AcceptMatchRequestCommand
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
            Assert.AreEqual(MatchRequestStatus.Accepted, result.Value.matchRequestStatus);
            Assert.AreEqual(matchProfile.MatchInitiatorUserId, result.Value.MatchInitiatorUserId);
        }

        [TestMethod]
        public async Task Handle_NotificationSent_VerifiesEventPublished()
        {
            // Arrange
            var command = new AcceptMatchRequestCommand
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
                It.Is<MatchAcceptedEvent>(e =>
                    e.UserId == command.UserId &&
                    e.MatchInitiatorUserId == matchProfile.MatchInitiatorUserId &&
                    e.AccepterUserId == command.UserId),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}