using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands;
using Fliq.Domain.Entities.DatingEnvironment;
using Fliq.Domain.Entities.Event.Enums;
using Moq;
using Fliq.Infrastructure.Persistence.Repositories;

namespace Fliq.Test.DatingEnvironment.Commands
{
    [TestClass]
    public class GetDatingListCommandHandlerTests
    {
        private Mock<ILoggerManager>? _mockLogger;
        private Mock<IBlindDateRepository>? _mockBlindDateRepository;
        private Mock<ISpeedDatingEventRepository>? _mockSpeedDatingEventRepository;
        private GetDatingListCommandHandler? _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILoggerManager>();
            _mockBlindDateRepository = new Mock<IBlindDateRepository>();
            _mockSpeedDatingEventRepository = new Mock<ISpeedDatingEventRepository>();
            _handler = new GetDatingListCommandHandler(
                _mockLogger.Object,
                _mockBlindDateRepository.Object,
                _mockSpeedDatingEventRepository.Object);
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsPaginatedDatingEvents()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 10,
                Title: "Test Event",
                Type: null, // Both types
                CreatedBy: "user1",
                SubscriptionType: "Premium",
                Duration: TimeSpan.FromHours(2),
                DateCreatedFrom: DateTime.Now.AddDays(-7),
                DateCreatedTo: DateTime.Now
            );

            var blindEvents = new List<DatingListItems>
            {
                new DatingListItems { Id = 1, Title = "Blind Date 1", Type = DatingType.BlindDating, CreatedBy = "User3", SubscriptionType = "Premium", Duration = TimeSpan.FromHours(2), DateCreated = DateTime.Now },
                new DatingListItems { Id = 2, Title = "Blind Date 2", Type = DatingType.BlindDating, CreatedBy = "User4", SubscriptionType = "Premium", Duration = TimeSpan.FromHours(2), DateCreated = DateTime.Now }
            };

            var speedEvents = new List<DatingListItems>
            {
                new DatingListItems { Id = 3, Title = "Speed Date 1", Type = DatingType.SpeedDating, CreatedBy = "User5", SubscriptionType = "Premium", Duration = TimeSpan.FromHours(2), DateCreated = DateTime.Now },
                new DatingListItems { Id = 4, Title = "Speed Date 2", Type = DatingType.SpeedDating, CreatedBy = "User6", SubscriptionType = "Premium", Duration = TimeSpan.FromHours(2), DateCreated = DateTime.Now }
            };

            var totalBlindCount = 2;
            var totalSpeedCount = 2;

            _mockBlindDateRepository?.Setup(r => r.GetAllFilteredListAsync(
                It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((blindEvents, totalBlindCount));

            _mockSpeedDatingEventRepository?.Setup(r => r.GetAllFilteredListAsync(
                It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((speedEvents, totalSpeedCount));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError); 
            //Assert.AreEqual(4, result.Value.Items.Count); // Both blind and speed events should be included
            Assert.AreEqual(4, result.Value.TotalCount); 
            Assert.AreEqual(command.Page, result.Value.PageNumber); 
            Assert.AreEqual(command.PageSize, result.Value.PageSize);
            _mockLogger?.Verify(l => l.LogInfo(It.IsAny<string>()), Times.AtLeastOnce()); 
        }

        [TestMethod]
        public async Task Handle_NoEventsFound_ReturnsNotFoundError()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 10,
                Title: "Nonexistent Event",
                Type: null,
                CreatedBy: null,
                SubscriptionType: null,
                Duration: null,
                DateCreatedFrom: null,
                DateCreatedTo: null
            );

            _mockBlindDateRepository?.Setup(r => r.GetAllFilteredListAsync(
                It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<DatingListItems>(), 0));

            _mockSpeedDatingEventRepository?.Setup(r => r.GetAllFilteredListAsync(
                It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<DatingListItems>(), 0));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError); 
            Assert.AreEqual("NoEventsFound", result.FirstError.Code); 
            _mockLogger?.Verify(l => l.LogInfo(It.IsAny<string>()), Times.Once()); 
            _mockLogger?.Verify(l => l.LogError("No dating events found matching the provided filters"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_RepositoryThrowsException_ReturnsFailureError()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 10,
                Title: "Test Event",
                Type: null,
                CreatedBy: "user1",
                SubscriptionType: "Premium",
                Duration: TimeSpan.FromHours(2),
                DateCreatedFrom: DateTime.Now.AddDays(-7),
                DateCreatedTo: DateTime.Now
            );

            _mockBlindDateRepository?.Setup(r => r.GetAllFilteredListAsync(
                It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            _mockSpeedDatingEventRepository?.Setup(r => r.GetAllFilteredListAsync(
                It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<DatingListItems>(), 0)); // Ensure no exception from speed dating

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError); 
            Assert.AreEqual("GetDatingListFailed", result.FirstError.Code); 
            Assert.AreEqual(ErrorType.Failure, result.FirstError.Type);
            _mockLogger?.Verify(l => l.LogError(It.IsAny<string>()), Times.Once()); 
        }

        [TestMethod]
        public async Task Handle_SpecificTypeFilter_ReturnsOnlyMatchingEvents()
        {
            // Arrange
            var command = new GetDatingListCommand(
                Page: 1,
                PageSize: 5,
                Title: null,
                Type: DatingType.BlindDating, // Only blind dating
                CreatedBy: null,
                SubscriptionType: null,
                Duration: null,
                DateCreatedFrom: null,
                DateCreatedTo: null
            );

            var blindEvents = new List<DatingListItems> 
            {
                    new DatingListItems { Id = 1, Title = "Blind Date 1", Type = DatingType.BlindDating, CreatedBy = "User3", SubscriptionType = "Premium", Duration = TimeSpan.FromHours(2), DateCreated = DateTime.Now }
            };

            _mockBlindDateRepository?.Setup(r => r.GetAllFilteredListAsync(
                It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((blindEvents, 1));

            _mockSpeedDatingEventRepository?.Setup(r => r.GetAllFilteredListAsync(
                It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan?>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<DatingListItems>(), 0)); // No speed dating events

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError); // No errors expected
            Assert.IsNotNull(result.Value); // Ensure value is not null
            Assert.IsTrue(result.Value.Data.Any()); // Ensure there are items in Data
            Assert.AreEqual(1, result.Value.Data.Count()); // Only one blind dating event
            Assert.IsTrue(result.Value.Data.All(e => e.Type == DatingType.BlindDating)); // All should be blind dating
            Assert.AreEqual(1, result.Value.TotalCount); // Total count should match
            Assert.AreEqual(command.Page, result.Value.PageNumber); // Page number should match
            Assert.AreEqual(command.PageSize, result.Value.PageSize); // Page size should match
        }
    }
}