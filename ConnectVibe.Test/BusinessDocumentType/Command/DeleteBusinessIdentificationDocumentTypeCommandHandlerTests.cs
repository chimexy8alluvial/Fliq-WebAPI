using ErrorOr;
using Fliq.Application.BusinessDocumentType.Command;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Moq;

namespace Fliq.Application.Tests.BusinessDocumentType.Command
{
    [TestClass]
    public class DeleteBusinessIdentificationDocumentTypeCommandHandlerTests
    {
        private Mock<IBusinessIdentificationDocumentTypeRepository>? _documentRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private DeleteBusinessIdentificationDocumentTypeCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _documentRepositoryMock = new Mock<IBusinessIdentificationDocumentTypeRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new DeleteBusinessIdentificationDocumentTypeCommandHandler(
                _documentRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidId_DeletesDocumentType_ReturnsDeleted()
        {
            // Arrange
            var command = new DeleteBusinessIdentificationDocumentTypeCommand(1);
            var documentType = new Fliq.Domain.Entities.BusinessIdentificationDocumentType { Id = 1, Name = "CAC Certificate", HasFrontAndBack = false };
            _documentRepositoryMock?.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(documentType);
            //_documentRepositoryMock?.Setup(r => r.IsInUseAsync(1)).ReturnsAsync(false);
            _documentRepositoryMock?.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Value, typeof(Deleted));
            _documentRepositoryMock?.Verify(r => r.GetByIdAsync(1), Times.Once());
            //_documentRepositoryMock?.Verify(r => r.IsInUseAsync(1), Times.Once());
            _documentRepositoryMock?.Verify(r => r.DeleteAsync(1), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Deleting document type with ID: 1"), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Deleted document type: CAC Certificate (ID: 1)"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_InvalidId_ReturnsNotFoundError()
        {
            // Arrange
            var command = new DeleteBusinessIdentificationDocumentTypeCommand(999);
            _documentRepositoryMock?.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Domain.Entities.BusinessIdentificationDocumentType)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.BusinessDocumentType.NotFound, result.FirstError);
            _documentRepositoryMock?.Verify(r => r.GetByIdAsync(999), Times.Once());
            //_documentRepositoryMock?.Verify(r => r.IsInUseAsync(It.IsAny<int>()), Times.Never());
            _documentRepositoryMock?.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never());
            _loggerMock?.Verify(l => l.LogInfo("Deleting document type with ID: 999"), Times.Once());
            _loggerMock?.Verify(l => l.LogWarn("Document type not found: ID 999"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_InUseDocumentType_ReturnsInUseError()
        {
            // Arrange
            var command = new DeleteBusinessIdentificationDocumentTypeCommand(1);
            var documentType = new Fliq.Domain.Entities.BusinessIdentificationDocumentType { Id = 1, Name = "CAC Certificate", HasFrontAndBack = false };
            _documentRepositoryMock?.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(documentType);
            //_documentRepositoryMock?.Setup(r => r.IsInUseAsync(1))
                //.ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.BusinessDocumentType.InUse, result.FirstError);
            _documentRepositoryMock?.Verify(r => r.GetByIdAsync(1), Times.Once());
            //_documentRepositoryMock?.Verify(r => r.IsInUseAsync(1), Times.Once());
            _documentRepositoryMock?.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never());
            _loggerMock?.Verify(l => l.LogInfo("Deleting document type with ID: 1"), Times.Once());
            _loggerMock?.Verify(l => l.LogWarn("Document type in use: ID 1"), Times.Once());
        }
    }
}