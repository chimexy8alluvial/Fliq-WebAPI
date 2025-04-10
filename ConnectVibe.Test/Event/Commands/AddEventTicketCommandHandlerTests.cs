using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.AddEventTicket;
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

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(It.IsAny<List<int>>()))
                .Returns(new List<Ticket>());

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Ticket.TicketNotFound, result.FirstError);
            _loggerMock!.Verify(x => x.LogError("Tickets not found for IDs: 1"), Times.Once());
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

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(It.IsAny<List<int>>()))
                .Returns(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns((Events?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
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

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(It.IsAny<List<int>>()))
                .Returns(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1))
                .Returns(new Events { Id = 1, Capacity = 10, Status = EventStatus.Upcoming, OccupiedSeats = new List<int>() });
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns((User?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommandWithMixedAssignments_AddsTicketsAndNotifiesCorrectly()
        {
            // Arrange
            var eventStartDate = DateTime.Now.AddDays(1);
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

            _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIds(It.IsAny<List<int>>())).Returns(tickets);
            _userRepositoryMock!.Setup(repo => repo.GetUserById(command.UserId)).Returns(buyer);
            _eventRepositoryMock!.Setup(repo => repo.GetEventById(1)).Returns(eventDetails);
            _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(command.PaymentId)).Returns(payment);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(5, eventDetails.OccupiedSeats.Count); // 2 initial + 3 new
            Assert.AreEqual(7, eventDetails.Capacity); // 10 - 3 tickets

            _eventRepositoryMock.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Once());
            _ticketRepositoryMock.Verify(repo => repo.UpdateRange(It.IsAny<List<Ticket>>()), Times.Once());
            _ticketRepositoryMock.Verify(repo => repo.AddEventTickets(It.IsAny<List<EventTicket>>()), Times.Once());
            _loggerMock!.Verify(logger => logger.LogInfo($"Assigned {3} tickets for event ID {1}."), Times.Once());

            // 1. Individual notification for User 200
            _mediatorMock!.Verify(x => x.Publish(
                It.Is<TicketPurchasedEvent>(e =>
                    e.SpecificUserId == 200 &&
                    e.SpecificTicketType == "Vip" &&
                    e.Message == $"A Vip ticket has been purchased for you for 'Test Event' on {eventStartDate:MMMM dd, yyyy} by John Doe." &&
                    e.BuyerId == 100 &&
                    e.OrganizerId == 300 &&
                    e.EventId == 1),
                It.IsAny<CancellationToken>()), Times.Once());

            // 2. Email notification for guest@example.com
            _mediatorMock.Verify(x => x.Publish(
                It.Is<TicketPurchasedEvent>(e =>
                    e.Email == "guest@example.com" &&
                    e.SpecificTicketType == "VVip" &&
                    e.Message == $"A VVip ticket has been purchased for you for 'Test Event' on {eventStartDate:MMMM dd, yyyy} by John Doe." &&
                    e.BuyerId == 100 &&
                    e.OrganizerId == 300 &&
                    e.EventId == 1),
                It.IsAny<CancellationToken>()), Times.Once());

            // 3. Buyer notification (summary)
            _mediatorMock.Verify(x => x.Publish(
                It.Is<TicketPurchasedEvent>(e =>
                    e.BuyerId == 100 &&
                    e.OrganizerId == 300 &&
                    e.EventId == 1 &&
                    e.NumberOfTickets == 3 &&
                    e.TicketDetails != null &&
                    e.TicketDetails.Count == 3 &&
                    e.Message == $"You have successfully purchased 3 ticket(s) for the event 'Test Event' on {eventStartDate:MMMM dd, yyyy}.\n" +
                                 "Breakdown: 1 Regular ticket, 1 Vip ticket, 1 VVip ticket\n" +
                                 "Assigned to: User 100: Regular, User 200: Vip, guest@example.com: VVip" &&
                    e.TicketDetails.Any(td => td.UserId == 100 && td.TicketType == "Regular") &&
                    e.TicketDetails.Any(td => td.UserId == 200 && td.TicketType == "Vip") &&
                    e.TicketDetails.Any(td => td.Email == "guest@example.com" && td.TicketType == "VVip")),
                It.IsAny<CancellationToken>()), Times.Once());

            // 4. No duplicate individual notification for User 100
            _mediatorMock.Verify(x => x.Publish(
                It.Is<TicketPurchasedEvent>(e =>
                    e.SpecificUserId == 100 &&
                    e.SpecificTicketType == "Regular"),
                It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}