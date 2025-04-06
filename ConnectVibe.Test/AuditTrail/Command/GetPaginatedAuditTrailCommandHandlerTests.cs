using ErrorOr;
using Fliq.Application.AuditTrail.Common;
using Fliq.Application.AuditTrailCommand;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Moq;

namespace Fliq.Test.AuditTrail.Commands
{
    [TestClass]
    public class GetPaginatedAuditTrailCommandHandlerTests
    {
        private Mock<IAuditTrailRepository>? _mockAuditTrailRepository;
        private Mock<ILoggerManager>? _mockLogger;
        private GetPaginatedAuditTrailCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockAuditTrailRepository = new Mock<IAuditTrailRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new GetPaginatedAuditTrailCommandHandler(_mockAuditTrailRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_AuditTrailsFound_ReturnsPaginationResponse()
        {
            // Arrange
            var command = new GetPaginatedAuditTrailCommand(1, 2, "John");

            var auditTrails = new List<AuditTrailListItem>
            {
                new AuditTrailListItem
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    AccessType = "Admin",
                    IPAddress = "192.168.1.1",
                    AuditAction = "Logged in"
                },
                new AuditTrailListItem
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    AccessType = "User",
                    IPAddress = "192.168.1.2",
                    AuditAction = "Logged out"
                }
            };
            var totalCount = 5; 

            _mockAuditTrailRepository?
                .Setup(repo => repo.GetAllAuditTrailsAsync(command.PageNumber, command.PageSize, command.Name))
                .ReturnsAsync((auditTrails, totalCount));

            _mockLogger?.Setup(l => l.LogInfo("Fetching all audit trails list"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError, "Expected no error when audit trails are found");
            Assert.IsNotNull(result.Value, "Expected a valid response");

            var response = result.Value;
            Assert.AreEqual(auditTrails.Count, response.Data.Count(), "Expected the same number of audit trails");
            Assert.AreEqual(totalCount, response.TotalCount, "Expected the correct total count");
            Assert.AreEqual(command.PageNumber, response.PageNumber, "Expected the correct page number");
            Assert.AreEqual(command.PageSize, response.PageSize, "Expected the correct page size");

            var firstTrail = response.Data.ElementAt(0);
            Assert.AreEqual(1, firstTrail.Id);
            Assert.AreEqual("John Doe", firstTrail.Name);
            Assert.AreEqual("john.doe@example.com", firstTrail.Email);
            Assert.AreEqual("Admin", firstTrail.AccessType);
            Assert.AreEqual("192.168.1.1", firstTrail.IPAddress);
            Assert.AreEqual("Logged in", firstTrail.AuditAction);

            _mockLogger?.Verify(l => l.LogInfo("Fetching all audit trails list"), Times.Once());
            _mockLogger?.Verify(l => l.LogError(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public async Task Handle_NoAuditTrailsFound_ReturnsEmptyPaginationResponse()
        {
            // Arrange
            var command = new GetPaginatedAuditTrailCommand(1, 10, null);

            var auditTrails = new List<AuditTrailListItem>(); // Empty list
            var totalCount = 0;

            _mockAuditTrailRepository?
                .Setup(repo => repo.GetAllAuditTrailsAsync(command.PageNumber, command.PageSize, command.Name))
                .ReturnsAsync((auditTrails, totalCount));

            _mockLogger?.Setup(l => l.LogInfo("Fetching all audit trails list"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError, "Expected no error when no audit trails are found");
            Assert.IsNotNull(result.Value, "Expected a valid response");

            var response = result.Value;
            Assert.AreEqual(0, response.Data.Count(), "Expected an empty list of audit trails");
            Assert.AreEqual(totalCount, response.TotalCount, "Expected total count to be 0");
            Assert.AreEqual(command.PageNumber, response.PageNumber, "Expected the correct page number");
            Assert.AreEqual(command.PageSize, response.PageSize, "Expected the correct page size");

            _mockLogger?.Verify(l => l.LogInfo("Fetching all audit trails list"), Times.Once());
            _mockLogger?.Verify(l => l.LogError(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public async Task Handle_ExceptionThrown_ReturnsError()
        {
            // Arrange
            var command = new GetPaginatedAuditTrailCommand(1, 10, "Test");

            _mockAuditTrailRepository?
                .Setup(repo => repo.GetAllAuditTrailsAsync(command.PageNumber, command.PageSize, command.Name))
                .ThrowsAsync(new Exception("Database error"));

            _mockLogger?.Setup(l => l.LogInfo("Fetching all audit trails list"));
            _mockLogger?.Setup(l => l.LogError(It.IsAny<string>())).Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError, "Expected an error when an exception is thrown");
            Assert.AreEqual("A failure has occurred.", result.FirstError.Description, "Expected the correct error message");
            Assert.AreEqual(ErrorType.Failure, result.FirstError.Type, "Expected a failure error type");

            _mockLogger?.Verify(l => l.LogInfo("Fetching all audit trails list"), Times.Once());
            _mockLogger?.Verify(l => l.LogError("Error fetching paginated audit trails: Database error"), Times.Once());
        }

        [TestMethod]
        public async Task Handle_RepositoryCalledWithCorrectParameters()
        {
            // Arrange
            var command = new GetPaginatedAuditTrailCommand(2, 5, "Jane");

            _mockAuditTrailRepository?
                .Setup(repo => repo.GetAllAuditTrailsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()))
                .ReturnsAsync((new List<AuditTrailListItem>(), 0));

            _mockLogger?.Setup(l => l.LogInfo("Fetching all audit trails list"));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockAuditTrailRepository?.Verify(repo =>
                repo.GetAllAuditTrailsAsync(
                    command.PageNumber,
                    command.PageSize,
                    command.Name),
                Times.Once(),
                "Repository should be called with the correct pagination and name parameters");

            _mockLogger?.Verify(l => l.LogInfo("Fetching all audit trails list"), Times.Once());
        }
    }
}