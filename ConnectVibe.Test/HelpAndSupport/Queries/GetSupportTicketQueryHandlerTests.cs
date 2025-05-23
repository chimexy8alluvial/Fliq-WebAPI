using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.HelpAndSupport.Queries.GetSupportTicket;
using Fliq.Domain.Entities.HelpAndSupport;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.HelpAndSupport.Queries
{
    [TestClass]
    public class GetSupportTicketQueryHandlerTests
    {
        // Mocks for dependencies
        private Mock<ISupportTicketRepository> _mockRepository;

        private Mock<ILoggerManager> _mockLogger;

        // Query handler to test
        private GetSupportTicketQueryHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mocks and handler before each test
            _mockRepository = new Mock<ISupportTicketRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new GetSupportTicketQueryHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_TicketFound_ReturnsTicket()
        {
            // Arrange
            var query = new GetSupportTicketQuery
            {
                SupportTicketId = "TICKET-1"
            };

            var expectedTicket = new SupportTicket
            {
                Id = 1,
                TicketId = "TICKET-1",
                Title = "Test Ticket",
                RequesterId = 1,
                RequesterName = "John Doe",
                RequestType = HelpRequestType.Billing,
                RequestStatus = HelpRequestStatus.Pending,
                IssueId = 1,
            };

            // Simulate ticket found in the repository
            _mockRepository
                .Setup(r => r.GetTicketBySupportTicketIdAsync(query.SupportTicketId))
                .ReturnsAsync(expectedTicket);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError); // Ensure no error is returned
            Assert.AreEqual(expectedTicket, result.Value); // Ensure the correct ticket is returned
            _mockLogger.Verify(l => l.LogError(It.IsAny<string>()), Times.Never); // Verify no error logging
        }

        [TestMethod]
        public async Task Handle_TicketNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var query = new GetSupportTicketQuery
            {
                SupportTicketId = "INVALID-TICKET"
            };

            // Simulate ticket not found in the repository
            _mockRepository
                .Setup(r => r.GetTicketBySupportTicketIdAsync(query.SupportTicketId))
                .ReturnsAsync((SupportTicket)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError); // Ensure an error is returned
            Assert.AreEqual(ErrorType.NotFound, result.FirstError.Type); // Ensure it's a "Not Found" error
            _mockLogger.Verify(l => l.LogError($"Ticket with SupportTicketId {query.SupportTicketId} not found."), Times.Once); // Verify error logging
        }
    }
}