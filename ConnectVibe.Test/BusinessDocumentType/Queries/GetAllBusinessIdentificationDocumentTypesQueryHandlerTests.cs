using Fliq.Application.BusinessDocumentType.Query;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Moq;

namespace Fliq.Application.Tests.BusinessDocumentType.Query
{
    [TestClass]
    public class GetAllBusinessIdentificationDocumentTypesQueryHandlerTests
    {
        private Mock<IBusinessIdentificationDocumentTypeRepository>? _documentRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetAllBusinessIdentificationDocumentTypesQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _documentRepositoryMock = new Mock<IBusinessIdentificationDocumentTypeRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllBusinessIdentificationDocumentTypesQueryHandler(_documentRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ReturnsAllDocumentTypes()
        {
            // Arrange
            var query = new GetAllBusinessIdentificationDocumentTypesQuery();
            var documentTypes = new List<Domain.Entities.BusinessIdentificationDocumentType>
            {
                new Fliq.Domain.Entities.BusinessIdentificationDocumentType { Id = 1, Name = "CAC Certificate", HasFrontAndBack = false },
                new Fliq.Domain.Entities.BusinessIdentificationDocumentType { Id = 2, Name = "Certificate Of Incorporation", HasFrontAndBack = true }
            };
            _documentRepositoryMock?.Setup(r => r.GetAllBusinessIdentificationDocumentTypesAsync()).Returns(documentTypes);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            //Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(2, result.Value.Count);
            Assert.AreEqual("CAC Certificate", result.Value[0].Name);
            Assert.IsFalse(result.Value[0].HasFrontAndBack);
            Assert.AreEqual("Certificate Of Incorporation", result.Value[1].Name);
            Assert.IsTrue(result.Value[1].HasFrontAndBack);
            _documentRepositoryMock?.Verify(r => r.GetAllBusinessIdentificationDocumentTypesAsync(), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Retrieving all business document types"), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Retrieved 2 document types"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetAllBusinessIdentificationDocumentTypesQuery();
            _documentRepositoryMock?.Setup(r => r.GetAllBusinessIdentificationDocumentTypesAsync()).Returns(new List<Domain.Entities.BusinessIdentificationDocumentType>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            //Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(0, result.Value.Count);
            _documentRepositoryMock?.Verify(r => r.GetAllBusinessIdentificationDocumentTypesAsync(), Times.Once());
            _loggerMock.Verify(l => l.LogInfo("Retrieving all business document types"), Times.Once());
            _loggerMock.Verify(l => l.LogInfo("Retrieved 0 document types"), Times.Once());
        }
    }
}
