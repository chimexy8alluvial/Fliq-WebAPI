using Moq;
using Fliq.Application.Event.Commands.Tickets;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MapsterMapper;

namespace Fliq.Test.Event.Commands
{
    [TestClass]
    public class AddTicketCommandHandlerTests
    {
        private Mock<IEventRepository> _eventRepositoryMock;
        private Mock<ITicketRepository> _ticketRepositoryMock;
        private Mock<ILoggerManager> _loggerMock;
        private Mock<IMapper> _mapperMock;

        private AddTicketCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _mapperMock = new Mock<IMapper>();

            _handler = new AddTicketCommandHandler(
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
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "VIP Ticket",
                TicketType = TicketType.Vip,
                Amount = 100.0m
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_AddsTicketSuccessfully()
        {
            // Arrange
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "General Admission",
                TicketType = TicketType.Regular,
                TicketDescription = "Standard entry ticket",
                EventDate = DateTime.UtcNow,
                CurrencyId = 1,
                Amount = 50.0m,
                MaximumLimit = "100",
                SoldOut = false
            };

            var eventEntity = new Events
            {
                Id = 1,
                Tickets = new List<Ticket>()
            };

            var ticket = new Ticket
            {
                TicketName = command.TicketName,
                TicketType = command.TicketType,
                Amount = command.Amount
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);
            _mapperMock.Setup(mapper => mapper.Map<Ticket>(command)).Returns(ticket);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _ticketRepositoryMock.Verify(repo => repo.Add(It.IsAny<Ticket>()), Times.Once);
            _loggerMock.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_DiscountsIncluded_AddsDiscountsToTicket()
        {
            // Arrange
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "Early Bird",
                TicketType = TicketType.Regular,
                TicketDescription = "Discounted ticket for early buyers",
                EventDate = DateTime.UtcNow,
                CurrencyId = 1,
                Amount = 30.0m,
                Discounts = new List<Discount>
                {
                    new Discount("10% off for early booking", 10.3, 4)
                }
            };

            var eventEntity = new Events { Id = 1, Tickets = new List<Ticket>() };
            var ticket = new Ticket
            {
                TicketName = command.TicketName,
                TicketType = command.TicketType,
                Amount = command.Amount,
                Discounts = command.Discounts
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);
            _mapperMock.Setup(mapper => mapper.Map<Ticket>(command)).Returns(ticket);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(command.Discounts.Count, ticket.Discounts?.Count);
            _ticketRepositoryMock.Verify(repo => repo.Add(It.IsAny<Ticket>()), Times.Once);
        }
    }
}