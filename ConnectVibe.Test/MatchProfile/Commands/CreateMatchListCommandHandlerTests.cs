using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.MatchProfile.Commands
{
    [TestClass]
    public class CreateMatchListCommandHandlerTests
    {
        private Mock<IMatchProfileRepository> _mockMatchProfileRepository;
        private Mock<ILoggerManager> _mockLogger;

        private CreateMatchListCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMatchProfileRepository = new Mock<IMatchProfileRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new CreateMatchListCommandHandler(_mockMatchProfileRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_NoMatchesFound_ReturnsEmptyList()
        {
            // Arrange
            var command = new GetMatchRequestListCommand(1, new PaginationRequest(1, 10), MatchRequestStatus.Pending);

            _mockMatchProfileRepository
                .Setup(repo => repo.GetMatchListById(It.IsAny<GetMatchListRequest>()))
                .ReturnsAsync(new List<MatchRequestDto>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Value.Count);
        }

        [TestMethod]
        public async Task Handle_MatchesFound_ReturnsMatchRequestDtoList()
        {
            // Arrange
            var command = new GetMatchRequestListCommand(1, new PaginationRequest(1, 10), MatchRequestStatus.Accepted);

            var matchRequestList = new List<MatchRequestDto>
            {
                new MatchRequestDto { MatchReceiverUserId = 1, MatchInitiatorUserId = 2},
                new MatchRequestDto { MatchInitiatorUserId = 3, MatchReceiverUserId = 1}
            };

            _mockMatchProfileRepository
                .Setup(repo => repo.GetMatchListById(It.IsAny<GetMatchListRequest>()))
                .ReturnsAsync(matchRequestList);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(2, result.Value.Count);
        }

        [TestMethod]
        public async Task Handle_RepositoryCalledWithCorrectParameters()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(1, 5);
            var command = new GetMatchRequestListCommand(1, paginationRequest, MatchRequestStatus.Rejected);

            _mockMatchProfileRepository
                .Setup(repo => repo.GetMatchListById(It.IsAny<GetMatchListRequest>()))
                .ReturnsAsync(new List<MatchRequestDto>());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMatchProfileRepository.Verify(repo =>
                repo.GetMatchListById(It.Is<GetMatchListRequest>(r =>
                    r.UserId == 1 &&
                    r.PaginationRequest.PageNumber == 1 &&
                    r.PaginationRequest.PageSize == 5 &&
                    r.MatchRequestStatus == MatchRequestStatus.Rejected)), Times.Once);
        }
    }
}