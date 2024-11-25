using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.UpdateTicket;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using Moq;

namespace Fliq.Test.Event.Commands
{
    [TestClass]
    public class UpdateTicketCommandHandlerTests
    {
        private Mock<IEventRepository> _eventRepositoryMock;
        private Mock<ITicketRepository> _ticketRepositoryMock;
        private Mock<ILoggerManager> _loggerMock;
        private Mock<IMapper> _mapperMock;

        private UpdateTicketCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _mapperMock = new Mock<IMapper>();

            _handler = new UpdateTicketCommandHandler(
                _eventRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _ticketRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new UpdateTicketCommand
            {
                EventId = 1,
                Id = 100,
                TicketName = "Updated Ticket Name"
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_TicketNotFound_ReturnsTicketNotFoundError()
        {
            // Arrange
            var command = new UpdateTicketCommand
            {
                EventId = 1,
                Id = 100,
                TicketName = "Updated Ticket Name"
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(command.EventId)).Returns(new Events { Id = command.EventId });
            _ticketRepositoryMock.Setup(repo => repo.GetTicketById(command.Id)).Returns((Ticket)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.TicketNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_UpdatesTicketSuccessfully()
        {
            // Arrange
            var command = new UpdateTicketCommand
            {
                EventId = 1,
                Id = 100,
                TicketName = "Updated Ticket Name",
                Amount = 75.50m
            };

            var existingEvent = new Events { Id = command.EventId };
            var existingTicket = new Ticket
            {
                Id = command.Id,
                EventId = command.EventId,
                TicketName = "Original Ticket Name",
                Amount = 50.00m
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(command.EventId)).Returns(existingEvent);
            _ticketRepositoryMock.Setup(repo => repo.GetTicketById(command.Id)).Returns(existingTicket);

            // Mock Mapster adaptation
            _mapperMock.Setup(mapper => mapper.Map<Ticket>(command)).Returns(existingTicket);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _ticketRepositoryMock.Verify(repo => repo.Update(It.Is<Ticket>(t =>
                t.Id == command.Id &&
                t.TicketName == command.TicketName &&
                t.Amount == command.Amount
            )), Times.Once);

            _loggerMock.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_PartialUpdate_OnlyUpdatesProvidedFields()
        {
            // Arrange
            var command = new UpdateTicketCommand
            {
                EventId = 1,
                Id = 100,
                TicketName = "Updated Partial Ticket Name" // Only updating the name
            };

            var existingEvent = new Events { Id = command.EventId };
            var existingTicket = new Ticket
            {
                Id = command.Id,
                EventId = command.EventId,
                TicketName = "Original Ticket Name",
                Amount = 50.00m
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(command.EventId)).Returns(existingEvent);
            _ticketRepositoryMock.Setup(repo => repo.GetTicketById(command.Id)).Returns(existingTicket);

            // Mock Mapster adaptation
            _mapperMock.Setup(mapper => mapper.Map<Ticket>(command)).Returns(existingTicket);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _ticketRepositoryMock.Verify(repo => repo.Update(It.Is<Ticket>(t =>
                t.Id == command.Id &&
                t.TicketName == command.TicketName &&
                t.Amount == existingTicket.Amount // Should remain unchanged
            )), Times.Once);

            _loggerMock.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
        }
    }
}