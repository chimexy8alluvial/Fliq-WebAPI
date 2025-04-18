﻿using Fliq.Application.BusinessDocumentType.Query;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Moq;

namespace Fliq.Application.Tests.BusinessDocumentType.Query
{
    [TestClass]
    public class GetAllBusinessDocumentTypesQueryHandlerTests
    {
        private Mock<IBusinessDocumentTypeRepository>? _documentRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private GetAllBusinessDocumentTypesQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _documentRepositoryMock = new Mock<IBusinessDocumentTypeRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new GetAllBusinessDocumentTypesQueryHandler(_documentRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ReturnsAllDocumentTypes()
        {
            // Arrange
            var query = new GetAllBusinessDocumentTypesQuery();
            var documentTypes = new List<Domain.Entities.BusinessDocumentType>
            {
                new Fliq.Domain.Entities.BusinessDocumentType { Id = 1, Name = "CAC Certificate", HasFrontAndBack = false },
                new Fliq.Domain.Entities.BusinessDocumentType { Id = 2, Name = "Certificate Of Incorporation", HasFrontAndBack = true }
            };
            _documentRepositoryMock?.Setup(r => r.GetAllBusinessDocumentTypesAsync()).Returns(documentTypes);

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
            _documentRepositoryMock?.Verify(r => r.GetAllBusinessDocumentTypesAsync(), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Retrieving all business document types"), Times.Once());
            _loggerMock?.Verify(l => l.LogInfo("Retrieved 2 document types"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetAllBusinessDocumentTypesQuery();
            _documentRepositoryMock?.Setup(r => r.GetAllBusinessDocumentTypesAsync()).Returns(new List<Domain.Entities.BusinessDocumentType>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            //Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(0, result.Value.Count);
            _documentRepositoryMock?.Verify(r => r.GetAllBusinessDocumentTypesAsync(), Times.Once());
            _loggerMock.Verify(l => l.LogInfo("Retrieving all business document types"), Times.Once());
            _loggerMock.Verify(l => l.LogInfo("Retrieved 0 document types"), Times.Once());
        }
    }
}
