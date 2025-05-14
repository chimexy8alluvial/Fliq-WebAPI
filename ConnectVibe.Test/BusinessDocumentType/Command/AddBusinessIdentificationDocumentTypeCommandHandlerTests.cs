using ErrorOr;
using Fliq.Application.BusinessDocumentType.Command;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Moq;

namespace Fliq.Application.Tests.BusinessIdentificationDocumentType.Command
{
    [TestClass]
    public class AddBusinessIdentificationDocumentTypeCommandHandlerTests
    {
        private Mock<IBusinessIdentificationDocumentTypeRepository>? _documentRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private AddBusinessIdentificationDocumentTypeCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _documentRepositoryMock = new Mock<IBusinessIdentificationDocumentTypeRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new AddBusinessIdentificationDocumentTypeCommandHandler(
                _documentRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidCommand_AddsDocumentType_ReturnsResponse()
        {
            // Arrange
            var command = new AddBusinessIdentificationDocumentTypeCommand("Business License", false);
            var documentType = new Fliq.Domain.Entities.BusinessIdentificationDocumentType { Id = 1, Name = "Business License", HasFrontAndBack = false };
            _documentRepositoryMock?.Setup(r => r.GetAllBusinessIdentificationDocumentTypesAsync()).Returns(new List<Domain.Entities.BusinessIdentificationDocumentType>());
            _documentRepositoryMock?.Setup(r => r.AddBusinessIdentificationDocumentTypeAsync(It.IsAny<Domain.Entities.BusinessIdentificationDocumentType>()))
                .Callback<Domain.Entities.BusinessIdentificationDocumentType>(dt => dt.Id = 1)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            Assert.AreEqual("Business License", result.Value.Name);
            Assert.IsFalse(result.Value.HasFrontAndBack);
            _documentRepositoryMock?.Verify(r => r.AddBusinessIdentificationDocumentTypeAsync(It.Is<Domain.Entities.BusinessIdentificationDocumentType>(dt => dt.Name == "Business License" && dt.HasFrontAndBack == false)), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Adding document type: Business License"), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Document type added successfully: Business License (ID: 1)"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_DuplicateName_ReturnsDuplicateNameError()
        {
            // Arrange
            var command = new AddBusinessIdentificationDocumentTypeCommand("Business License", false);
            var existing = new List<Domain.Entities.BusinessIdentificationDocumentType>
            {
                new Fliq.Domain.Entities.BusinessIdentificationDocumentType { Id = 1, Name = "Business License", HasFrontAndBack = false }
            };
            _documentRepositoryMock?.Setup(r => r.GetAllBusinessIdentificationDocumentTypesAsync()).Returns(existing);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.BusinessDocumentType.DuplicateName, result.FirstError);
            _documentRepositoryMock?.Verify(r => r.AddBusinessIdentificationDocumentTypeAsync(It.IsAny<Domain.Entities.BusinessIdentificationDocumentType>()), Times.Never());
            _loggerMock?.Verify(l => l.LogInfo("Adding document type: Business License"), Times.Once());
            _loggerMock?.Verify(l => l.LogWarn("Document type already exists: Business License"), Times.Once());
        }
    }
}
