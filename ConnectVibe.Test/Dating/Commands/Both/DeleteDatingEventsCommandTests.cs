using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands;
using Fliq.Application.DatingEnvironment.Common;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using Fliq.Domain.Common.Errors;
using MediatR;
using Moq;

namespace Fliq.Test.Dating.Commands.Both
{
    [TestClass]
    public class DeleteDatingEventsCommandTests
    {
        private Mock<IBlindDateRepository>? _mockBlindDateRepository;
        private Mock<ISpeedDatingEventRepository>? _mockSpeedDatingEventRepository;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private Mock<IMediator>? _mockMediator;
        private DeleteDatingEventsCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockBlindDateRepository = new Mock<IBlindDateRepository>();
            _mockSpeedDatingEventRepository = new Mock<ISpeedDatingEventRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();
            _mockMediator = new Mock<IMediator>();

            _handler = new DeleteDatingEventsCommandHandler(
                _mockBlindDateRepository.Object,
                _mockSpeedDatingEventRepository.Object,
                _mockMediator.Object,
                _mockLoggerManager.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_DeletesEventsSuccessfully()
        {
            // Arrange
            var datingOptions = new List<DatingOptions>
            {
                new DatingOptions { id = 1, DatingType = DatingType.BlindDating },
                new DatingOptions { id = 2, DatingType = DatingType.SpeedDating }
            };
            var command = new DeleteDatingEventsCommand(datingOptions);

            _mockBlindDateRepository?
                .Setup(repo => repo.DeleteMultipleAsync(new List<int> { 1 }))
                .ReturnsAsync(1);

            _mockSpeedDatingEventRepository?
                .Setup(repo => repo.DeleteMultipleAsync(new List<int> { 2 }))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(2, result.Value.TotalDeletedCount);
            Assert.AreEqual(1, result.Value.BlindDateDeletedCount);
            Assert.AreEqual(1, result.Value.SpeedDateDeletedCount);

            _mockBlindDateRepository?.Verify(repo => repo.DeleteMultipleAsync(It.Is<List<int>>(ids => ids.Contains(1))), Times.Once);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.DeleteMultipleAsync(It.Is<List<int>>(ids => ids.Contains(2))), Times.Once);
            _mockLoggerManager?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains($"Total dating events seleted: {result.Value.TotalDeletedCount}"))), Times.Once);
        }
         
        [TestMethod]
        public async Task Handle_NullOrEmptyDatingOptions_ReturnsError()
        {
            // Arrange
            var command = new DeleteDatingEventsCommand(null); // Null DatingOptions

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.NoEventsToDelete.Description, result.Errors.First().Description);

            _mockBlindDateRepository?.Verify(repo => repo.DeleteMultipleAsync(It.IsAny<List<int>>()), Times.Never);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.DeleteMultipleAsync(It.IsAny<List<int>>()), Times.Never);
            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("No dating options provided for deletion"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_NoEventsDeleted_ReturnsError()
        {
            // Arrange
            var datingOptions = new List<DatingOptions>
            {
                new DatingOptions { id = 1, DatingType = DatingType.BlindDating },
                new DatingOptions { id = 2, DatingType = DatingType.SpeedDating }
            };
            var command = new DeleteDatingEventsCommand(datingOptions);

            _mockBlindDateRepository?
                .Setup(repo => repo.DeleteMultipleAsync(new List<int> { 1 }))
                .ReturnsAsync(0);

            _mockSpeedDatingEventRepository?
                .Setup(repo => repo.DeleteMultipleAsync(new List<int> { 2 }))
                .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Dating.NoEventsToDelete.Description, result.Errors.First().Description);

            _mockBlindDateRepository?.Verify(repo => repo.DeleteMultipleAsync(It.Is<List<int>>(ids => ids.Contains(1))), Times.Once);
            _mockSpeedDatingEventRepository?.Verify(repo => repo.DeleteMultipleAsync(It.Is<List<int>>(ids => ids.Contains(2))), Times.Once);
            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("No dating events were deleted"))), Times.Once);
        }

        //[TestMethod]
        //public async Task Handle_PartialDeletion_ReturnsSuccessWithCorrectCounts()
        //{
        //    // Arrange
        //    var datingOptions = new List<DatingOptions>
        //    {
        //        new DatingOptions { id = 1, DatingType = DatingType.BlindDating },
        //        new DatingOptions { id = 2, DatingType = DatingType.SpeedDating }
        //    };
        //    var command = new DeleteDatingEventsCommand(datingOptions);

        //    _mockBlindDateRepository?
        //        .Setup(repo => repo.DeleteMultipleAsync(new List<int> { 1 }))
        //        .ReturnsAsync(1);

        //    _mockSpeedDatingEventRepository?
        //        .Setup(repo => repo.DeleteMultipleAsync(new List<int> { 2 }))
        //        .ReturnsAsync(0);

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.IsFalse(result.IsError);
        //    Assert.IsNotNull(result.Value);
        //    Assert.AreEqual(1, result.Value.TotalDeletedCount);
        //    Assert.AreEqual(1, result.Value.BlindDateDeletedCount);
        //    Assert.AreEqual(0, result.Value.SpeedDateDeletedCount);

        //    _mockBlindDateRepository?.Verify(repo => repo.DeleteMultipleAsync(It.Is<List<int>>(ids => ids.Contains(1))), Times.Once);
        //    _mockSpeedDatingEventRepository?.Verify(repo => repo.DeleteMultipleAsync(It.Is<List<int>>(ids => ids.Contains(2))), Times.Once);
        //    _mockLoggerManager?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Total dating events deleted: 1"))), Times.Once);
        //}
    }
}




