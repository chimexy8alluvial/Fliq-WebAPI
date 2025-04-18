
using Fliq.Application.BusinessDocumentType.Query;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Moq;

namespace Fliq.Application.Tests.BusinessDocumentType.Query
{
    [TestClass]
    public class GetBusinessDocumentTypeByIdQueryHandlerTests
    {
        private Mock<IBusinessDocumentTypeRepository>? _documentRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetBusinessDocumentTypeByIdQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _documentRepositoryMock = new Mock<IBusinessDocumentTypeRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetBusinessDocumentTypeByIdQueryHandler(
                _documentRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidId_ReturnsDocumentType()
        {
            // Arrange
            var query = new GetBusinessDocumentTypeByIdQuery(1);
            var documentType = new Fliq.Domain.Entities.BusinessDocumentType { Id = 1, Name = "CAC Certificate", HasFrontAndBack = false };
            _documentRepositoryMock?.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(documentType);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            Assert.AreEqual("CAC Certificate", result.Value.Name);
            Assert.IsFalse(result.Value.HasFrontAndBack);
            _documentRepositoryMock?.Verify(r => r.GetByIdAsync(1), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Retrieving document type with ID: 1"), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Retrieved document type: CAC Certificate (ID: 1)"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_InvalidId_ReturnsNotFoundError()
        {
            // Arrange
            var query = new GetBusinessDocumentTypeByIdQuery(999);
            _documentRepositoryMock?.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Domain.Entities.BusinessDocumentType)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.BusinessDocumentType.NotFound, result.FirstError);
            _documentRepositoryMock?.Verify(r => r.GetByIdAsync(999), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Retrieving document type with ID: 999"), Times.Once());
            _loggerMock?.Verify(l => l.LogWarn("Document type not found: ID 999"), Times.Once());
        }
    }
}







