using ErrorOr;
using Fliq.Application.BusinessDocumentType.Command;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Moq;

namespace Fliq.Application.Tests.BusinessDocumentType.Command
{
    [TestClass]
    public class AddBusinessDocumentTypeCommandHandlerTests
    {
        private Mock<IBusinessDocumentTypeRepository>? _documentRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private AddBusinessDocumentTypeCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _documentRepositoryMock = new Mock<IBusinessDocumentTypeRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new AddBusinessDocumentTypeCommandHandler(
                _documentRepositoryMock.Object, 
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidCommand_AddsDocumentType_ReturnsResponse()
        {
            // Arrange
            var command = new AddBusinessDocumentTypeCommand("Business License", false);
            var documentType = new Fliq.Domain.Entities.BusinessDocumentType { Id = 1, Name = "Business License", HasFrontAndBack = false };
            _documentRepositoryMock?.Setup(r => r.GetAllBusinessDocumentTypesAsync()).Returns(new List<Domain.Entities.BusinessDocumentType>());
            _documentRepositoryMock?.Setup(r => r.AddBusinessDocumentTypeAsync(It.IsAny<Domain.Entities.BusinessDocumentType>()))
                .Callback<Domain.Entities.BusinessDocumentType>(dt => dt.Id = 1)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            Assert.AreEqual("Business License", result.Value.Name);
            Assert.IsFalse(result.Value.HasFrontAndBack);
            _documentRepositoryMock?.Verify(r => r.AddBusinessDocumentTypeAsync(It.Is<Domain.Entities.BusinessDocumentType>(dt => dt.Name == "Business License" && dt.HasFrontAndBack == false)), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Adding document type: Business License"), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Document type added successfully: Business License (ID: 1)"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_DuplicateName_ReturnsDuplicateNameError()
        {
            // Arrange
            var command = new AddBusinessDocumentTypeCommand("Business License", false);
            var existing = new List<Domain.Entities.BusinessDocumentType>
            {
                new Fliq.Domain.Entities.BusinessDocumentType { Id = 1, Name = "Business License", HasFrontAndBack = false }
            };
            _documentRepositoryMock?.Setup(r => r.GetAllBusinessDocumentTypesAsync()).Returns(existing);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.BusinessDocumentType.DuplicateName, result.FirstError);
            _documentRepositoryMock?.Verify(r => r.AddBusinessDocumentTypeAsync(It.IsAny<Domain.Entities.BusinessDocumentType>()), Times.Never());
            _loggerMock?.Verify(l => l.LogInfo("Adding document type: Business License"), Times.Once());
            _loggerMock?.Verify(l => l.LogWarn("Document type already exists: Business License"), Times.Once());
        }
    }
}
