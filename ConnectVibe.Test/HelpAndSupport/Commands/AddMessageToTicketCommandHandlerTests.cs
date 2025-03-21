using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.HelpAndSupport.Commands.AddMessage;
using Fliq.Domain.Entities.HelpAndSupport;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.HelpAndSupport.Commands
{
    [TestClass]
    public class AddMessageToTicketCommandHandlerTests
    {
        // Mocks for dependencies
        private Mock<ISupportTicketRepository> _mockRepository;

        private Mock<ILoggerManager> _mockLogger;

        // Command handler to test
        private AddMessageToTicketCommandHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mocks and handler before each test
            _mockRepository = new Mock<ISupportTicketRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _handler = new AddMessageToTicketCommandHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_TicketNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var command = new AddMessageToTicketCommand
            {
                SupportTicketId = "TICKET-1",
                SenderId = 1,
                SenderName = "John Doe",
                Content = "Test message"
            };

            // Simulate ticket not found in the repository
            _mockRepository
                .Setup(r => r.GetTicketBySupportTicketIdAsync(command.SupportTicketId))
                .ReturnsAsync((SupportTicket)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError); // Ensure an error is returned
            Assert.AreEqual(ErrorType.NotFound, result.FirstError.Type); // Ensure it's a "Not Found" error
            _mockLogger.Verify(l => l.LogError($"Ticket with SupportTicketId {command.SupportTicketId} not found."), Times.Once); // Verify error logging
        }

        [TestMethod]
        public async Task Handle_ValidCommand_ReturnsMessageId()
        {
            // Arrange
            var command = new AddMessageToTicketCommand
            {
                SupportTicketId = "TICKET-1",
                SenderId = 1,
                SenderName = "John Doe",
                Content = "Test message"
            };

            var ticket = new SupportTicket
            {
                Id = 1,
                TicketId = "TICKET-1",
                Title = "Test Ticket",
                RequesterId = 1,
                RequesterName = "John Doe",
                RequestType = HelpRequestType.Billing,
                RequestStatus = HelpRequestStatus.Pending
            };

            // Simulate ticket found in the repository
            _mockRepository
                .Setup(r => r.GetTicketBySupportTicketIdAsync(command.SupportTicketId))
                .ReturnsAsync(ticket);

            // Simulate adding a message and returning a message ID
            _mockRepository
                .Setup(r => r.AddMessageToTicketAsync(ticket.Id, It.IsAny<HelpMessage>()))
                .Callback<int, HelpMessage>((ticketId, message) => message.Id = 123); // Simulate message ID generation

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError); // Ensure no error is returned
            Assert.AreEqual(123, result.Value); // Ensure the correct message ID is returned
            _mockLogger.Verify(l => l.LogInfo($"Message added to ticket with SupportTicketId {command.SupportTicketId}."), Times.Once); // Verify success logging
        }
    }
}