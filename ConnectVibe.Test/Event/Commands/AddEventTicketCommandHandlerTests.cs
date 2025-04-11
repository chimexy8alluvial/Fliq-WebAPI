using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.AddEventTicket;
using Fliq.Application.Event.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Contracts.Event;
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
        public async Task Handle_NoValidTicketIds_ReturnsNoTicketsSpecifiedError()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                UserId = 100,
                PurchaseTicketDetail = new List<PurchaseTicketDetail>
                {
                    new PurchaseTicketDetail { UserId = 100, TicketId = 0 } // Invalid TicketId
                },
                PaymentId = 200
            };

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.NoTicketsSpecified, result.FirstError);
            _loggerMock!.Verify(x => x.LogError("No valid ticket IDs provided in the command."), Times.Once());
        }

        [TestMethod]
        public async Task Handle_TicketNotFound_ReturnsTicketNotFoundError()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                UserId = 100,
                PurchaseTicketDetail = new List<PurchaseTicketDetail>
                {
                    new PurchaseTicketDetail { UserId = 100, TicketId = 1 }
                },
                PaymentId = 200
            };

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.TicketNotFound, result.FirstError);
            _loggerMock!.Verify(x => x.LogError("Tickets not found for IDs: 1."), Times.Once());
            _ticketRepositoryMock.Verify(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }), Times.Once());
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                UserId = 100,
                PurchaseTicketDetail = new List<PurchaseTicketDetail>
                {
                    new PurchaseTicketDetail { UserId = 100, TicketId = 1 }
                },
                PaymentId = 200
            };

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
            _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1)).ReturnsAsync((Events?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
            _loggerMock!.Verify(x => x.LogError("Event with ID 1 not found."), Times.Once());
            _ticketRepositoryMock.Verify(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }), Times.Once());
            _eventRepositoryMock.Verify(repo => repo.GetEventByIdAsync(1), Times.Once());
        }

        [TestMethod]
        public async Task Handle_BuyerNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new AddEventTicketCommand
            {
                UserId = 100,
                PurchaseTicketDetail = new List<PurchaseTicketDetail>
                {
                    new PurchaseTicketDetail { UserId = 100, TicketId = 1 }
                },
                PaymentId = 200
            };

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
            _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
                .ReturnsAsync(new Events { Id = 1, Capacity = 10, Status = EventStatus.Upcoming, OccupiedSeats = new List<int>() });
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns((User?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
            _loggerMock!.Verify(x => x.LogError("Buyer with ID 100 not found."), Times.Once());
            _ticketRepositoryMock.Verify(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }), Times.Once());
            _eventRepositoryMock.Verify(repo => repo.GetEventByIdAsync(1), Times.Once());
            _userRepositoryMock.Verify(repo => repo.GetUserById(100), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ValidCommandWithMixedAssignments_AddsTicketsAndNotifiesCorrectly()
        {
            // Arrange
            var eventStartDate = DateTime.Now.AddDays(1).Date; // Normalize to midnight
            var command = new AddEventTicketCommand
            {
                UserId = 100, // Buyer
                PurchaseTicketDetail = new List<PurchaseTicketDetail>
                {
                    new PurchaseTicketDetail { UserId = 100, TicketId = 1 }, // Buyer
                    new PurchaseTicketDetail { UserId = 200, TicketId = 2 }, // Another user
                    new PurchaseTicketDetail { UserId = null, Email = "guest@example.com", TicketId = 3 } // Guest via email
                },
                PaymentId = 200
            };

            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, SoldOut = false, TicketType = TicketType.Regular },
                new Ticket { Id = 2, EventId = 1, SoldOut = false, TicketType = TicketType.Vip },
                new Ticket { Id = 3, EventId = 1, SoldOut = false, TicketType = TicketType.VVip }
            };
            var eventDetails = new Events
            {
                Id = 1,
                Capacity = 10,
                OccupiedSeats = new List<int> { 1, 2 },
                EventTitle = "Test Event",
                StartDate = eventStartDate,
                UserId = 300, // Organizer
                Status = EventStatus.Upcoming
            };
            var buyer = new User { Id = 100, DisplayName = "John Doe" };
            var payment = new Payment { Id = 200 };

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(tickets);
            _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1)).ReturnsAsync(eventDetails);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(buyer);
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            _ticketRepositoryMock.Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Ticket>>())).Returns(Task.CompletedTask);
            _ticketRepositoryMock.Setup(repo => repo.AddEventTicketsAsync(It.IsAny<List<EventTicket>>())).Returns(Task.CompletedTask);
            _eventRepositoryMock.Setup(repo => repo.Update(It.IsAny<Events>())); // Synchronous Update

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(CreateEventTicketResult));
            Assert.AreEqual(5, eventDetails.OccupiedSeats.Count); // 2 initial + 3 new
            Assert.AreEqual(7, eventDetails.Capacity); // 10 - 3 tickets

            _eventRepositoryMock.Verify(repo => repo.Update(It.Is<Events>(e => e.Id == 1)), Times.Once());
            _ticketRepositoryMock.Verify(repo => repo.UpdateRangeAsync(It.Is<List<Ticket>>(t => t.Count == 3)), Times.Once());
            _ticketRepositoryMock.Verify(repo => repo.AddEventTicketsAsync(It.Is<List<EventTicket>>(et => et.Count == 3)), Times.Once());
            _loggerMock!.Verify(logger => logger.LogInfo($"Assigned {3} tickets for event ID {1}."), Times.Once());

            // 1. Individual notification for User 200
            _mediatorMock!.Verify(x => x.Publish(
                It.Is<TicketPurchasedEvent>(e =>
                    e.SpecificUserId == 200 &&
                    e.SpecificTicketType == "Vip" &&
                    e.BuyerId == 100 &&
                    e.OrganizerId == 300 &&
                    e.EventId == 1 &&
                    e.Title == "Your Ticket Purchase Confirmation" &&
                    e.Message.Contains("A Vip ticket has been purchased for you for 'Test Event'") &&
                    e.Message.Contains("by John Doe")),
                It.IsAny<CancellationToken>()), Times.Once());

            // 2. Email notification for guest@example.com
            _mediatorMock.Verify(x => x.Publish(
                It.Is<TicketPurchasedEvent>(e =>
                    e.Email == "guest@example.com" &&
                    e.SpecificTicketType == "VVip" &&
                    e.BuyerId == 100 &&
                    e.OrganizerId == 300 &&
                    e.EventId == 1 &&
                    e.Title == "Your Ticket Purchase Confirmation" &&
                    e.Message.Contains("A VVip ticket has been purchased for you for 'Test Event'") &&
                    e.Message.Contains("by John Doe")),
                It.IsAny<CancellationToken>()), Times.Once());

            // 3. Buyer-specific notification for their own ticket
            _mediatorMock.Verify(x => x.Publish(
                It.Is<TicketPurchasedEvent>(e =>
                    e.SpecificUserId == 100 &&
                    e.SpecificTicketType == "Regular" &&
                    e.BuyerId == 100 &&
                    e.OrganizerId == 300 &&
                    e.EventId == 1 &&
                    e.Title == "Your Personal Ticket Confirmation" &&
                    e.Message.Contains("You have been assigned 1 ticket(s) for 'Test Event'") &&
                    e.Message.Contains("Regular")),
                It.IsAny<CancellationToken>()), Times.Once());

            // 4. General buyer notification (summary)
            _mediatorMock.Verify(x => x.Publish(
                It.Is<TicketPurchasedEvent>(e =>
                    e.BuyerId == 100 &&
                    e.OrganizerId == 300 &&
                    e.EventId == 1 &&
                    e.NumberOfTickets == 3 &&
                    e.TicketDetails != null &&
                    e.TicketDetails.Count == 3 &&
                    e.Title == "New Tickets Purchased" &&
                    e.Message.Contains("You have successfully purchased 3 ticket(s) for the event 'Test Event'") &&
                    e.Message.Contains("Breakdown: 1 Regular ticket, 1 Vip ticket, 1 VVip ticket") &&
                    e.Message.Contains("Assigned to: User 100: Regular, User 200: Vip, guest@example.com: VVip") &&
                    e.TicketDetails.Any(td => td.UserId == 100 && td.TicketType == "Regular") &&
                    e.TicketDetails.Any(td => td.UserId == 200 && td.TicketType == "Vip") &&
                    e.TicketDetails.Any(td => td.Email == "guest@example.com" && td.TicketType == "VVip")),
                It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}