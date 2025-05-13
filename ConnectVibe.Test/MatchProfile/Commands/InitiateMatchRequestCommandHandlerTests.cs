using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Commands.Create;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;
using Moq;

namespace Fliq.Test.MatchProfile.Commands
{
    [TestClass]
    public class InitiateMatchRequestCommandHandlerTests
    {
        private Mock<IMapper>? _mockMapper;
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<IMatchProfileRepository>? _mockMatchProfileRepository;
        private Mock<IMediator>? _mockMediator;
        private Mock<ILoggerManager>? _mockLogger;
        private InitiateMatchRequestCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMatchProfileRepository = new Mock<IMatchProfileRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILoggerManager>();

            _handler = new InitiateMatchRequestCommandHandler(
                _mockMapper.Object,
                _mockUserRepository.Object,
                _mockMatchProfileRepository.Object,
                _mockMediator.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new InitiateMatchRequestCommand
            {
                MatchInitiatorUserId = 1,
                MatchReceiverUserId = 2
            };

            _mockUserRepository?
                .Setup(repo => repo.GetUserById(command.MatchReceiverUserId))
                .Returns((Domain.Entities.User?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.User.UserNotFound));
        }

        [TestMethod]
        public async Task Handle_SuccessfulMatchCreation_ReturnsCreateMatchProfileResult()
        {
            // Arrange
            var command = new InitiateMatchRequestCommand
            {
                MatchInitiatorUserId = 1,
                MatchReceiverUserId = 2
            };

            var matchInitiator = new User
            {
                Id = 1,
                FirstName = "John",
                UserProfile = new UserProfile
                {
                    DOB = new DateTime(1990, 1, 1),
                    Photos = new List<ProfilePhoto> { new ProfilePhoto { PictureUrl = "http://image.com/john.jpg", Caption = "John" } }
                }
            };

            var matchReceiver = new User
            {
                Id = 2,
                UserProfile = new UserProfile
                {
                    Photos = new List<ProfilePhoto> { new ProfilePhoto { PictureUrl = "http://image.com/jane.jpg", Caption = "Jane" } }
                }
            };

            var matchRequest = new MatchRequest
            {
                MatchInitiatorUserId = command.MatchInitiatorUserId,
                MatchReceiverUserId = command.MatchReceiverUserId,
                MatchRequestStatus = MatchRequestStatus.Pending
            };

            _mockUserRepository?
                .Setup(repo => repo.GetUserByIdIncludingProfile(command.MatchReceiverUserId))
                .Returns(matchReceiver);

            _mockUserRepository?
                .Setup(repo => repo.GetUserByIdIncludingProfile(command.MatchInitiatorUserId))
                .Returns(matchInitiator);

            _mockMapper?
                .Setup(mapper => mapper.Map<MatchRequest>(command))
                .Returns(matchRequest);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(command.MatchInitiatorUserId, result.Value.MatchInitiatorUserId);
            Assert.AreEqual("John ", result.Value.Name);
            Assert.AreEqual("http://image.com/john.jpg", result.Value.PictureUrl);
        }

        [TestMethod]
        public async Task Handle_NotificationSent_VerifiesEventPublished()
        {
            // Arrange
            var command = new InitiateMatchRequestCommand
            {
                MatchInitiatorUserId = 1,
                MatchReceiverUserId = 2
            };

            var matchInitiator = new Domain.Entities.User
            {
                Id = 1,
                FirstName = "John",
                UserProfile = new UserProfile
                {
                    DOB = new DateTime(1990, 1, 1),
                    Photos = new List<ProfilePhoto> { new ProfilePhoto { PictureUrl = "http://image.com/john.jpg", Caption = "John" } }
                }
            };

            var matchReceiver = new Domain.Entities.User
            {
                Id = 2,
                UserProfile = new UserProfile
                {
                    Photos = new List<ProfilePhoto> { new ProfilePhoto { PictureUrl = "http://image.com/jane.jpg", Caption = "Jane" } }
                }
            };

            var matchRequest = new MatchRequest
            {
                MatchInitiatorUserId = command.MatchInitiatorUserId,
                MatchReceiverUserId = command.MatchReceiverUserId,
                MatchRequestStatus = MatchRequestStatus.Pending
            };

            _mockUserRepository?
                .Setup(repo => repo.GetUserByIdIncludingProfile(command.MatchReceiverUserId))
                .Returns(matchReceiver);

            _mockUserRepository?
                .Setup(repo => repo.GetUserByIdIncludingProfile(command.MatchInitiatorUserId))
                .Returns(matchInitiator);

            _mockMapper?
                .Setup(mapper => mapper.Map<MatchRequest>(command))
                .Returns(matchRequest);

            // Setup to capture the published event
            MatchRequestEvent capturedEvent = null!;
            _mockMediator?
                .Setup(m => m.Publish(It.IsAny<MatchRequestEvent>(), It.IsAny<CancellationToken>()))
                .Callback<object, CancellationToken>((notification, ct) =>
                {
                    capturedEvent = notification as MatchRequestEvent;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _handler!.Handle(command, CancellationToken.None);

            // Assert
            await Task.Delay(100); // Small delay to ensure async publication completes
            Assert.IsNotNull(capturedEvent, "MatchRequestEvent was not published.");
            Assert.AreEqual(command.MatchInitiatorUserId, capturedEvent.UserId);
            Assert.AreEqual(command.MatchReceiverUserId, capturedEvent.AccepterUserId);
            Assert.AreEqual(command.MatchInitiatorUserId, capturedEvent.InitiatorUserId);
            Assert.AreEqual("http://image.com/jane.jpg", capturedEvent.AccepterImageUrl);
            Assert.AreEqual("http://image.com/john.jpg", capturedEvent.InitiatorImageUrl);
            Assert.AreEqual("John ", capturedEvent.InitiatorName); // Note: Space due to FirstName + " " + LastName (LastName is null)
        }
    }
}