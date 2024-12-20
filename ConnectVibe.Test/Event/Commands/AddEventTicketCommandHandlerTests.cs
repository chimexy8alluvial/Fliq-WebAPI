using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.AddEventTicket;
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

        private AddEventTicketCommandHandler _handler;

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
            var command = new AddEventTicketCommand { TicketId = 1, UserId = 100, PaymentId = 200, NumberOfTickets = 2 };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_InsufficientCapacity_ReturnsInsufficientCapacityError()
        {
            // Arrange
            var command = new AddEventTicketCommand { TicketId = 1, UserId = 100, PaymentId = 200, NumberOfTickets = 5 };

            var user = new User { Id = command.UserId };
            _userRepositoryMock?.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            var eventDetails = new Events { Id = 1, Capacity = 3, OccupiedSeats = new List<int>() };
            _eventRepositoryMock?.Setup(repo => repo.GetEventById(command.TicketId)).Returns(eventDetails);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.InsufficientCapacity, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_PaymentNotFound_ReturnsPaymentNotFoundError()
        {
            // Arrange
            var command = new AddEventTicketCommand { TicketId = 1, UserId = 100, PaymentId = 200, NumberOfTickets = 2 };
            var user = new User { Id = command.UserId };
            _userRepositoryMock?.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            var eventDetails = new Events { Id = 1, Capacity = 10, OccupiedSeats = new List<int>() };
            _eventRepositoryMock?.Setup(repo => repo.GetEventById(command.TicketId)).Returns(eventDetails);

            _paymentRepositoryMock?.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns((Payment)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Payment.PaymentNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_NoAvailableSeats_ReturnsNoAvailableSeatsError()
        {
            // Arrange
            var command = new AddEventTicketCommand { TicketId = 1, UserId = 100, PaymentId = 200, NumberOfTickets = 5 };

            var eventDetails = new Events
            {
                Id = 1,
                Capacity = 10,
                OccupiedSeats = Enumerable.Range(1, 10).ToList() // All seats are occupied
            };
            var user = new User { Id = command.UserId };
            _userRepositoryMock?.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _eventRepositoryMock?.Setup(repo => repo.GetEventById(command.TicketId)).Returns(eventDetails);

            var payment = new Payment { Id = command.PaymentId };
            _paymentRepositoryMock?.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.NoAvailableSeats, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_AddsTicketsSuccessfully()
        {
            // Arrange
            var command = new AddEventTicketCommand { TicketId = 1, UserId = 100, PaymentId = 200, NumberOfTickets = 3 };

            var eventDetails = new Events
            {
                Id = 1,
                Capacity = 10,
                OccupiedSeats = new List<int> { 1, 2 }
            };

            var payment = new Payment { Id = command.PaymentId };
            var user = new User { Id = command.UserId };
            _userRepositoryMock?.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _eventRepositoryMock?.Setup(repo => repo.GetEventById(command.TicketId)).Returns(eventDetails);
            _paymentRepositoryMock?.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            var newTickets = new List<EventTicket>();
            _ticketRepositoryMock?.Setup(repo => repo.AddEventTicket(It.IsAny<EventTicket>()))
                                 .Callback<EventTicket>(ticket => newTickets.Add(ticket));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(3, newTickets.Count);
            Assert.AreEqual(5, eventDetails.OccupiedSeats.Count); // 3 new seats added to existing 2
            Assert.AreEqual(7, eventDetails.Capacity); // Capacity reduced by 3

            _eventRepositoryMock?.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Once);
            _loggerMock?.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
        }
    }
}