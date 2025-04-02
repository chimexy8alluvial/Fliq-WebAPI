using ErrorOr;
using Fliq.Application.Commands;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.Event;
using MediatR;
using Moq;

namespace Fliq.Application.Tests.Commands
{
    [TestClass]
    public class RefundTicketCommandHandlerTests
    {
        private Mock<ITicketRepository>? _ticketRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private RefundTicketCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _handler = new RefundTicketCommandHandler(_ticketRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ValidTicketId_ReturnsUnit()
        {
            // Arrange
            int ticketId = 1;
            var ticket = new Ticket { Id = ticketId, IsRefunded = false };
            var command = new RefundTicketCommand(ticketId);

            _ticketRepositoryMock!
                .Setup(x => x.GetTicketById(ticketId))
                .Returns(ticket);
            _ticketRepositoryMock
                .Setup(x => x.Update(It.IsAny<Ticket>()));
               

            _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(Unit.Value, result.Value);

            _loggerMock.Verify(x => x.LogInfo($"Processing refund request for TicketId: {ticketId}"), Times.Once());
            _loggerMock.Verify(x => x.LogInfo($"Successfully refunded TicketId: {ticketId}"), Times.Once());
            _ticketRepositoryMock.Verify(x => x.GetTicketById(ticketId), Times.Once());
            _ticketRepositoryMock.Verify(x => x.Update(It.Is<Ticket>(t => t.Id == ticketId && t.IsRefunded)), Times.Once());
        }

        [TestMethod]
        public async Task Handle_TicketNotFound_ReturnsNotFoundError()
        {
            // Arrange
            int ticketId = 2;
            var command = new RefundTicketCommand(ticketId);

            _ticketRepositoryMock!
                .Setup(x => x.GetTicketById(ticketId))
                .Returns((Ticket)null!);

            _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
            _loggerMock.Setup(x => x.LogWarn(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(ErrorType.NotFound, result.FirstError.Type);
            Assert.AreEqual("TicketNotFound", result.FirstError.Code);
            Assert.AreEqual($"Ticket with ID {ticketId} does not exist.", result.FirstError.Description);

            _loggerMock.Verify(x => x.LogInfo($"Processing refund request for TicketId: {ticketId}"), Times.Once());
            _loggerMock.Verify(x => x.LogWarn($"Ticket with TicketId {ticketId} not found."), Times.Once());
            _ticketRepositoryMock.Verify(x => x.GetTicketById(ticketId), Times.Once());
            _ticketRepositoryMock.Verify(x => x.Update(It.IsAny<Ticket>()), Times.Never());
        }

        [TestMethod]
        public async Task Handle_AlreadyRefundedTicket_ReturnsConflictError()
        {
            // Arrange
            int ticketId = 3;
            var ticket = new Ticket { Id = ticketId, IsRefunded = true };
            var command = new RefundTicketCommand(ticketId);

            _ticketRepositoryMock!
                .Setup(x => x.GetTicketById(ticketId))
                .Returns(ticket);

            _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
            _loggerMock.Setup(x => x.LogWarn(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(ErrorType.Conflict, result.FirstError.Type);
            Assert.AreEqual("TicketAlreadyRefunded", result.FirstError.Code);
            Assert.AreEqual($"Ticket with ID {ticketId} has already been refunded.", result.FirstError.Description);

            _loggerMock.Verify(x => x.LogInfo($"Processing refund request for TicketId: {ticketId}"), Times.Once());
            _loggerMock.Verify(x => x.LogWarn($"Ticket with TicketId {ticketId} is already refunded."), Times.Once());
            _ticketRepositoryMock.Verify(x => x.GetTicketById(ticketId), Times.Once());
            _ticketRepositoryMock.Verify(x => x.Update(It.IsAny<Ticket>()), Times.Never());
        }

        [TestMethod]
        public async Task Handle_RepositoryThrowsException_ReturnsFailureError()
        {
            // Arrange
            int ticketId = 4;
            var command = new RefundTicketCommand(ticketId);
            var exception = new Exception("Database error");

            _ticketRepositoryMock!
                .Setup(x => x.GetTicketById(ticketId))
                .Throws(exception);

            _loggerMock!.Setup(x => x.LogInfo(It.IsAny<string>()));
            _loggerMock.Setup(x => x.LogError(It.IsAny<string>()));

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(ErrorType.Failure, result.FirstError.Type);
            Assert.AreEqual("RefundTicketFailed", result.FirstError.Code);
            Assert.IsTrue(result.FirstError.Description.Contains("Database error"));

            _loggerMock.Verify(x => x.LogInfo($"Processing refund request for TicketId: {ticketId}"), Times.Once());
            _loggerMock.Verify(x => x.LogError($"Error refunding TicketId {ticketId}: {exception.Message}"), Times.Once());
            _ticketRepositoryMock.Verify(x => x.GetTicketById(ticketId), Times.Once());
            _ticketRepositoryMock.Verify(x => x.Update(It.IsAny<Ticket>()), Times.Never());
        }
    }
}