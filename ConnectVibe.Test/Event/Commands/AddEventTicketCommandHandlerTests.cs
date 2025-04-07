using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.AddEventTicket;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
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
        public async Task Handle_NoTicketIds_ReturnsNoTicketsSpecifiedError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int>(), UserId = 100, PaymentId = 200 };
            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.NoTicketsSpecified, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_TicketNotFound_ReturnsTicketNotFoundError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1, 2, 3 }, UserId = 100, PaymentId = 200 };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId))
                .Returns(new List<Ticket> { new Ticket { Id = 1, EventId = 1 } });

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.TicketNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_MultipleEvents_ReturnsMultipleEventsNotSupportedError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1, 2 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 2, EventId = 2, TicketType = TicketType.Regular, SoldOut = false }
            };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.MultipleEventsNotSupported, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false } };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns((Events)null!);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_EventDeleted_ReturnsEventNotFoundError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false } };
            var eventDetails = new Events { Id = 1, IsDeleted = true, Status = EventStatus.Upcoming };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_EventCancelled_ReturnsEventCancelledAlreadyError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false } };
            var eventDetails = new Events { Id = 1, Status = EventStatus.Cancelled, IsDeleted = false };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventCancelledAlready, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_EventPast_ReturnsEventEndedAlreadyError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false } };
            var eventDetails = new Events { Id = 1, Status = EventStatus.Past, IsDeleted = false };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventEndedAlready, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false } };
            var eventDetails = new Events { Id = 1, Capacity = 10, IsDeleted = false, Status = EventStatus.Upcoming };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns((User)null!);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_InsufficientCapacity_ReturnsInsufficientCapacityError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1, 2, 3 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 2, EventId = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 3, EventId = 1, TicketType = TicketType.Vip, SoldOut = false }
            };
            var eventDetails = new Events { Id = 1, Capacity = 2, OccupiedSeats = new List<int>(), IsDeleted = false, Status = EventStatus.Upcoming };
            var user = new User { Id = command.UserId };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.InsufficientCapacity, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_TicketsSoldOut_ReturnsTicketAlreadySoldOutError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1, 2 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = true },
                new Ticket { Id = 2, EventId = 1, TicketType = TicketType.Regular, SoldOut = false }
            };
            var eventDetails = new Events { Id = 1, Capacity = 10, OccupiedSeats = new List<int>(), IsDeleted = false, Status = EventStatus.Upcoming };
            var user = new User { Id = command.UserId };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.TicketAlreadySoldOut, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_PaymentNotFound_ReturnsPaymentNotFoundError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false } };
            var eventDetails = new Events { Id = 1, Capacity = 10, OccupiedSeats = new List<int>(), IsDeleted = false, Status = EventStatus.Upcoming };
            var user = new User { Id = command.UserId };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns((Payment)null!);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Payment.PaymentNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_NoAvailableSeats_ReturnsNoAvailableSeatsError()
        {
            var command = new AddEventTicketCommand { TicketId = new List<int> { 1, 2, 3 }, UserId = 100, PaymentId = 200 };
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 2, EventId = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 3, EventId = 1, TicketType = TicketType.Vip, SoldOut = false }
            };
            var eventDetails = new Events { Id = 1, Capacity = 5, OccupiedSeats = new List<int> { 1, 2, 3, 4, 5 }, IsDeleted = false, Status = EventStatus.Upcoming };
            var user = new User { Id = command.UserId };
            var payment = new Payment { Id = command.PaymentId };
            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.NoAvailableSeats, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommandWithUpcomingStatus_AddsTicketsSuccessfully()
        {
            var command = new AddEventTicketCommand
            {
                TicketId = new List<int> { 1, 2, 3 },
                UserId = 100,
                PaymentId = 200
            };
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 2, EventId = 1, TicketType = TicketType.Regular, SoldOut = false },
                new Ticket { Id = 3, EventId = 1, TicketType = TicketType.Vip, SoldOut = false }
            };
            var eventDetails = new Events
            {
                Id = 1,
                Capacity = 10,
                OccupiedSeats = new List<int> { 1, 2 },
                EventTitle = "Test Event",
                StartDate = DateTime.Now.AddDays(1), // Upcoming event
                UserId = 99,
                IsDeleted = false,
                Status = EventStatus.Upcoming
            };
            var user = new User { Id = command.UserId, DisplayName = "Test User" };
            var payment = new Payment { Id = command.PaymentId };

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsError);
            Assert.AreEqual(3, result.Value.EventTickets.Count);
            Assert.AreEqual(5, eventDetails.OccupiedSeats.Count);
            Assert.AreEqual(7, eventDetails.Capacity);

            _ticketRepositoryMock.Verify(repo => repo.UpdateRange(It.Is<IEnumerable<Ticket>>(ts => ts.Count() == 3)), Times.Once);
            _ticketRepositoryMock.Verify(repo => repo.AddEventTickets(It.Is<IEnumerable<EventTicket>>(ets => ets.Count() == 3)), Times.Once);
            _eventRepositoryMock.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Once);
            _loggerMock!.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
            _mediatorMock!.Verify(m => m.Publish(
                It.Is<TicketPurchasedEvent>(e => e.Message.Contains("Breakdown: 2 Regular tickets, 1 Vip ticket")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ValidCommandWithOngoingStatus_AddsTicketsSuccessfully()
        {
            var command = new AddEventTicketCommand
            {
                TicketId = new List<int> { 1 },
                UserId = 100,
                PaymentId = 200
            };
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false }
            };
            var eventDetails = new Events
            {
                Id = 1,
                Capacity = 10,
                OccupiedSeats = new List<int> { 1 },
                EventTitle = "Ongoing Event",
                StartDate = DateTime.Now.AddDays(-1), // Started but ongoing
                UserId = 99,
                IsDeleted = false,
                Status = EventStatus.Ongoing
            };
            var user = new User { Id = command.UserId, DisplayName = "Test User" };
            var payment = new Payment { Id = command.PaymentId };

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1, result.Value.EventTickets.Count);
            Assert.AreEqual(2, eventDetails.OccupiedSeats.Count);
            Assert.AreEqual(9, eventDetails.Capacity);

            _ticketRepositoryMock.Verify(repo => repo.UpdateRange(It.Is<IEnumerable<Ticket>>(ts => ts.Count() == 1)), Times.Once);
            _ticketRepositoryMock.Verify(repo => repo.AddEventTickets(It.Is<IEnumerable<EventTicket>>(ets => ets.Count() == 1)), Times.Once);
            _eventRepositoryMock.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_PendingApprovalStatus_AddsTicketsSuccessfully()
        {
            var command = new AddEventTicketCommand
            {
                TicketId = new List<int> { 1 },
                UserId = 100,
                PaymentId = 200
            };
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, TicketType = TicketType.Regular, SoldOut = false }
            };
            var eventDetails = new Events
            {
                Id = 1,
                Capacity = 10,
                OccupiedSeats = new List<int> { 1 },
                EventTitle = "Pending Event",
                StartDate = DateTime.Now.AddDays(5),
                UserId = 99,
                IsDeleted = false,
                Status = EventStatus.PendingApproval
            };
            var user = new User { Id = command.UserId, DisplayName = "Test User" };
            var payment = new Payment { Id = command.PaymentId };

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(command.TicketId)).Returns(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(user);
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            var result = await _handler!.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1, result.Value.EventTickets.Count);
            Assert.AreEqual(2, eventDetails.OccupiedSeats.Count);
            Assert.AreEqual(9, eventDetails.Capacity);
        }
    }
}