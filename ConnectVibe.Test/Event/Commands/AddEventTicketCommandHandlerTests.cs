using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.AddEventTicket;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using MediatR;
using Moq;

namespace Fliq.Test.Event.Commands
{
    [TestClass]
    public class AddEventTicketCommandHandlerTests
    {
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ITicketRepository>? _ticketRepositoryMock;
        private Mock<IPaymentRepository>? _paymentRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IMediator>? _mediatorMock;

        private AddEventTicketCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AddEventTicketCommandHandler(
                _eventRepositoryMock.Object,
                _loggerMock.Object,
                _ticketRepositoryMock.Object,
                _paymentRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mediatorMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                EventId = 1,
                UserId = 100,
                PaymentId = 200,
                TicketQuantities = new Dictionary<TicketType, int> { { TicketType.Regular, 2 } }
            };

            _eventRepositoryMock!.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null!);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_InsufficientCapacity_ReturnsInsufficientCapacityError()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                EventId = 1,
                UserId = 100,
                PaymentId = 200,
                TicketQuantities = new Dictionary<TicketType, int> { { TicketType.Regular, 5 } }
            };

            var user = new User { Id = command.UserId };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            var eventDetails = new Events { Id = 1, Capacity = 3, OccupiedSeats = new List<int>() };
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(command.EventId)).Returns(eventDetails);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.InsufficientCapacity, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_PaymentNotFound_ReturnsPaymentNotFoundError()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                EventId = 1,
                UserId = 100,
                PaymentId = 200,
                TicketQuantities = new Dictionary<TicketType, int> { { TicketType.Regular, 2 } }
            };

            var user = new User { Id = command.UserId };
            var eventDetails = new Events { Id = 1, Capacity = 10, OccupiedSeats = new List<int>() };
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 2, TicketType = TicketType.Regular, SoldOut = false }
            };

            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(command.EventId)).Returns(eventDetails);
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByEventId(command.EventId)).Returns(tickets); // Fix: Return valid tickets
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns((Payment)null!);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Payment.PaymentNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_AllTicketsSoldOut_ReturnsTicketAlreadySoldOutError()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                EventId = 1,
                UserId = 100,
                PaymentId = 200,
                TicketQuantities = new Dictionary<TicketType, int> { { TicketType.Regular, 2 } }
            };

            var user = new User { Id = command.UserId };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            var eventDetails = new Events { Id = 1, Capacity = 10, OccupiedSeats = new List<int>() };
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(command.EventId)).Returns(eventDetails);

            var soldOutTickets = new List<Ticket>
            {
                new Ticket { Id = 1, TicketType = TicketType.Regular, SoldOut = true }
            };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByEventId(command.EventId)).Returns(soldOutTickets);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.TicketAlreadySoldOut, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_AddsTicketsSuccessfully()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                EventId = 1,
                UserId = 100,
                PaymentId = 200,
                TicketQuantities = new Dictionary<TicketType, int>
                {
                    { TicketType.Regular, 2 },
                    { TicketType.Vip, 1 }
                }
            };

            var eventDetails = new Events
            {
                Id = 1,
                Capacity = 10,
                OccupiedSeats = new List<int> { 1, 2 },
                EventTitle = "Test Event",
                StartDate = DateTime.Now
            };

            var payment = new Payment { Id = command.PaymentId };
            var user = new User { Id = command.UserId, DisplayName = "Test User" };
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(command.EventId)).Returns(eventDetails);
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            var availableTickets = new List<Ticket>
            {
                new Ticket { Id = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 2, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 3, TicketType = TicketType.Vip, SoldOut = false }
            };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByEventId(command.EventId)).Returns(availableTickets);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(3, result.Value.EventTickets.Count);  // Updated to EventTickets
            Assert.AreEqual(5, eventDetails.OccupiedSeats.Count); // 3 new seats added to existing 2
            Assert.AreEqual(7, eventDetails.Capacity); // Capacity reduced by 3

            _ticketRepositoryMock.Verify(repo => repo.Update(It.IsAny<Ticket>()), Times.Exactly(3));
            _eventRepositoryMock.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Once);
            _loggerMock!.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
            _mediatorMock!.Verify(m => m.Publish(It.IsAny<TicketPurchasedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

   
}