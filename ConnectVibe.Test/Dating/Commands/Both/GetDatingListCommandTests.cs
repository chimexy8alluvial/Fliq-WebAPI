using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands;
using Fliq.Contracts.Dating;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using Moq;
using System;

namespace Fliq.Test.Dating.Commands.Both
{
    [TestClass]
    public class GetDatingListCommandTests
    {
        private Mock<ILoggerManager>? _mockLoggerManager;
        private Mock<ISpeedDatingEventRepository>? _mockSpeedDatingEventRepository;
        private Mock<IBlindDateRepository>? _mockBlindDateRepository;
        private GetDatingListCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockLoggerManager = new Mock<ILoggerManager>();
            _mockBlindDateRepository = new Mock<IBlindDateRepository>();
            _mockSpeedDatingEventRepository = new Mock<ISpeedDatingEventRepository>();

            _handler = new GetDatingListCommandHandler(
                _mockLoggerManager.Object,
                _mockSpeedDatingEventRepository.Object,
                _mockBlindDateRepository.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequestWithDateRange_ReturnsDatingEventsSuccessfully()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 2,
                Title: "Test",
                Type: null,
                CreatedBy: "user1",
                SubscriptionType: null,
                Duration: null,
                DateCreatedFrom: new DateTime(2025, 3, 1),
                DateCreatedTo: new DateTime(2025, 3, 31)
            );

            var blindDates = new List<DatingListItem>
            {
                new DatingListItem { Title = "Blind Date 1", Type = DatingType.BlindDating, DateCreated = new DateTime(2025, 3, 27), CreatedBy = "user1", SubscriptionType = "Premium User", Duration = TimeSpan.FromHours(2) }
            };
            var speedDates = new List<DatingListItem>
            {
                new DatingListItem { Title = "Speed Date 1", Type = DatingType.SpeedDating, DateCreated = new DateTime(2025, 3, 27), CreatedBy = "user1", SubscriptionType = "Premium User", Duration = TimeSpan.FromHours(1) }
            };

            _mockBlindDateRepository?
                .Setup(repo => repo.GetAllFilteredListAsync(
                    "Test", null, null, null, new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), "user1", 1, 2))
                .ReturnsAsync((blindDates, 1));

            _mockSpeedDatingEventRepository?
                .Setup(repo => repo.GetAllFilteredListAsync(
                    "Test", null, null, null, new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), "user1", 1, 2))
                .ReturnsAsync((speedDates, 1));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(2, result.Value.List.Count);
            Assert.AreEqual(2, result.Value.TotalCount);
            Assert.AreEqual("Blind Date 1", result.Value.List[0].Title);
            Assert.AreEqual("Speed Date 1", result.Value.List[1].Title);

            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync("Test", null, null, null, new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), "user1", 1, 2), Times.Once);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync("Test", null, null, null, new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), "user1", 1, 2), Times.Once);
            _mockLoggerManager?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Retrieved 2 events out of 2 total matching filters"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_InvalidPage_ReturnsError()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 0, 
                PageSize: 10,
                Title: null,
                Type: null,
                CreatedBy: null,
                SubscriptionType: null,
                Duration: null,
                DateCreatedFrom: null,
                DateCreatedTo: null
            );

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.InvalidPaginationPage.Description, result.FirstError.Description);

            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Invalid page number provided"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_NoEventsFound_ReturnsError()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 10,
                Title: "NonExistent",
                Type: null,
                CreatedBy: "user1",
                SubscriptionType: null,
                Duration: null,
                DateCreatedFrom: new DateTime(2025, 3, 1),
                DateCreatedTo: new DateTime(2025, 3, 31)
            );

            _mockBlindDateRepository?
                .Setup(repo => repo.GetAllFilteredListAsync(
                    "NonExistent", null, null, null, new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), "user1", 1, 10))
                .ReturnsAsync((new List<DatingListItem>(), 0));

            _mockSpeedDatingEventRepository?
                .Setup(repo => repo.GetAllFilteredListAsync(
                    "NonExistent", null, null, null, new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), "user1", 1, 10))
                .ReturnsAsync((new List<DatingListItem>(), 0));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.NoEventsFound.Description, result.FirstError.Description);

            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync("NonExistent", null, null, null, new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), "user1", 1, 10), Times.Once);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync("NonExistent", null, null, null, new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), "user1", 1, 10), Times.Once);
            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("No dating events found matching the provided filters"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_InvalidDateRangeOrder_ReturnsError()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 10,
                Title: null,
                Type: null,
                CreatedBy: null,
                SubscriptionType: null,
                Duration: null,
                DateCreatedFrom: new DateTime(2025, 12, 31), // From > To
                DateCreatedTo: new DateTime(2025, 1, 1)
            );

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.InvalidDateRangeOrder.Description, result.FirstError.Description);

            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Invalid date range"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_InvalidDateCreatedFrom_ReturnsError()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 10,
                Title: null,
                Type: null,
                CreatedBy: null,
                SubscriptionType: null,
                Duration: null,
                DateCreatedFrom: new DateTime(1700, 1, 1), // Before SQL Server min (1753)
                DateCreatedTo: new DateTime(2025, 12, 31)
            );

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.InvalidDateCreatedFromRange.Description, result.FirstError.Description);

            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Invalid DateCreatedFrom"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_InvalidDuration_ReturnsError()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 10,
                Title: null,
                Type: null,
                CreatedBy: null,
                SubscriptionType: null,
                Duration: TimeSpan.FromHours(-1), // Negative duration
                DateCreatedFrom: new DateTime(2025, 1, 1),
                DateCreatedTo: new DateTime(2025, 12, 31)
            );

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.InvalidDuration.Description, result.FirstError.Description);

            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Invalid Duration"))), Times.Once);
        }
    }
}