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
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ITicketRepository>? _ticketRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IMapper>? _mapperMock;

        private UpdateTicketCommandHandler? _handler;

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

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null);

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

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(command.EventId)).Returns(new Events { Id = command.EventId });
            _ticketRepositoryMock?.Setup(repo => repo.GetTicketById(command.Id)).Returns((Ticket)null);

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
                Amount = 75.50m,
                Discounts = new List<Discount>
                {
                   new Discount
                    {
                        Name = "5% off for booking in the next 7 days",
                        Percentage = 10.3,
                        NumberOfTickets = 4
                    }
                },
            };

            var existingEvent = new Events { Id = command.EventId };
            var existingTicket = new Ticket
            {
                Id = command.Id,
                EventId = command.EventId,
                TicketName = "Original Ticket Name",
                Amount = 50.00m,
                Discounts = new List<Discount>
                {
                    new Discount
                    {
                        Name = "5% off for booking in the next 7 days",
                        Percentage = 10.3,
                        NumberOfTickets = 4
                    }
                },
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(command.EventId)).Returns(existingEvent);
            _ticketRepositoryMock?.Setup(repo => repo.GetTicketById(command.Id)).Returns(existingTicket);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _ticketRepositoryMock?.Verify(repo => repo.Update(It.Is<Ticket>(t =>
                t.Id == command.Id &&
                t.TicketName == command.TicketName &&
                t.Amount == command.Amount
            )), Times.Once);

            _loggerMock?.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
        }
    }
}