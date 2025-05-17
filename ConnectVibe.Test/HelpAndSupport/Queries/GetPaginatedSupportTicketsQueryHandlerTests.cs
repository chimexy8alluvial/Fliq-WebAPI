using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.HelpAndSupport.Queries.GetSupportTickets;
using Fliq.Domain.Entities.HelpAndSupport;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.HelpAndSupport.Queries
{
    [TestClass]
    public class GetPaginatedSupportTicketsQueryHandlerTests
    {
        // Mocks for dependencies
        private Mock<ISupportTicketRepository> _mockRepository;

        private Mock<ILoggerManager> _mockLogger;

        // Query handler to test
        private GetPaginatedSupportTicketsQueryHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mocks and handler before each test
            _mockRepository = new Mock<ISupportTicketRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new GetPaginatedSupportTicketsQueryHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsPaginatedTickets()
        {
            // Arrange
            var query = new GetPaginatedSupportTicketsQuery
            {
                PaginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 }
            };

            var tickets = new List<SupportTicket>
        {
            new SupportTicket
            {
                Id = 1,
                TicketId = "TICKET-1",
                Title = "Test Ticket 1",
                RequesterId = 1,
                RequesterName = "John Doe",
                RequestType = HelpRequestType.Billing,
                RequestStatus = HelpRequestStatus.Pending,
                GameSessionId = 1,
            },
            new SupportTicket
            {
                Id = 2,
                TicketId = "TICKET-2",
                Title = "Test Ticket 2",
                RequesterId = 2,
                RequesterName = "Jane Smith",
                RequestType = HelpRequestType.Billing,
                RequestStatus = HelpRequestStatus.Pending,
                GameSessionId = 2,
            }
        };

            var totalTickets = 20;

            // Simulate fetching paginated tickets from the repository
            _mockRepository
                .Setup(r => r.GetPaginatedSupportTicketsAsync(query.PaginationRequest, query.RequestType, query.RequestStatus))
                .ReturnsAsync(tickets);

            // Simulate getting the total count of tickets
            _mockRepository
                .Setup(r => r.GetTotalSupportTicketsCountAsync(query.RequestType, query.RequestStatus))
                .ReturnsAsync(totalTickets);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError); // Ensure no error is returned
            Assert.AreEqual(totalTickets, result.Value.TotalCount); // Ensure the correct total count is returned
            Assert.AreEqual(query.PaginationRequest.PageNumber, result.Value.PageNumber); // Ensure the correct page number is returned
            Assert.AreEqual(query.PaginationRequest.PageSize, result.Value.PageSize); // Ensure the correct page size is returned
            _mockLogger.Verify(l => l.LogError(It.IsAny<string>()), Times.Never); // Verify no error logging
        }

        [TestMethod]
        public async Task Handle_RepositoryThrowsException_ReturnsFailureError()
        {
            // Arrange
            var query = new GetPaginatedSupportTicketsQuery
            {
                PaginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 }
            };

            // Simulate repository throwing an exception
            _mockRepository
                .Setup(r => r.GetPaginatedSupportTicketsAsync(query.PaginationRequest, query.RequestType, query.RequestStatus))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError); // Ensure an error is returned
            Assert.AreEqual(ErrorType.Failure, result.FirstError.Type); // Ensure it's a "Failure" error
            _mockLogger.Verify(l => l.LogError("Error fetching paginated support tickets: Database error"), Times.Once); // Verify error logging
        }
    }
}