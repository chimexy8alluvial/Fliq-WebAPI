using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.HelpAndSupport.Commands.Create;
using Fliq.Domain.Entities.HelpAndSupport;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.HelpAndSupport.Commands
{
    [TestClass]
    public class CreateSupportTicketCommandHandlerTests
    {
        // Mocks for dependencies
        private Mock<ISupportTicketRepository> _mockRepository;

        private Mock<ILoggerManager> _mockLogger;

        // Command handler to test
        private CreateSupportTicketCommandHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mocks and handler before each test
            _mockRepository = new Mock<ISupportTicketRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new CreateSupportTicketCommandHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_ReturnsTicketId()
        {
            // Arrange
            var command = new CreateSupportTicketCommand
            {
                Title = "Test Ticket",
                RequesterId = 1,
                RequesterName = "John Doe",
                RequestType = HelpRequestType.Billing
            };

            var expectedTicketId = "TICKET-1";
            var supportTicket = new SupportTicket { TicketId = expectedTicketId };
            // Simulate ticket creation in the repository
            _mockRepository
                .Setup(r => r.CreateTicketAsync(It.IsAny<SupportTicket>()))
                .ReturnsAsync(supportTicket);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError); // Ensure no error is returned
            Assert.AreEqual(expectedTicketId, result.Value.SupportTicketId); // Ensure the correct ticket ID is returned
            _mockLogger.Verify(l => l.LogInfo("Help request command received"), Times.Once); // Verify initial logging
            _mockLogger.Verify(l => l.LogInfo($"Help request created with ID {expectedTicketId}"), Times.Once); // Verify success logging
        }

        [TestMethod]
        public async Task Handle_RepositoryThrowsException_ReturnsError()
        {
            // Arrange
            var command = new CreateSupportTicketCommand
            {
                Title = "Test Ticket",
                RequesterId = 1,
                RequesterName = "John Doe",
                RequestType = HelpRequestType.Billing
            };

            // Simulate repository throwing an exception
            _mockRepository
                .Setup(r => r.CreateTicketAsync(It.IsAny<SupportTicket>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError); // Ensure an error is returned
            Assert.AreEqual(ErrorType.Failure, result.FirstError.Type); // Ensure it's a "Failure" error
            _mockLogger.Verify(l => l.LogInfo("Help request command received"), Times.Once); // Verify initial logging
            _mockLogger.Verify(l => l.LogError(It.IsAny<string>()), Times.Once); // Verify error logging
        }
    }
}