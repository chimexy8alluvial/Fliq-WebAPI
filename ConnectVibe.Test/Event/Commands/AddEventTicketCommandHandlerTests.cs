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

[TestClass]
public class AddEventTicketCommandHandlerTests
{
    private Mock<IEventRepository>? _eventRepositoryMock;
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<IPaymentRepository>? _paymentRepositoryMock;
    private Mock<IUserRepository>? _userRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private Mock<IMediator>? _mediatorMock;
    private AddEventTicketCommandHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILoggerManager>();
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
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 0 } // Invalid ticket ID
            },
            PaymentId = 1
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
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Ticket>());

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketNotFound, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Tickets not found for IDs: 1."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_TicketAlreadySoldOut_ReturnsTicketAlreadySoldOutError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = true } });

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketAlreadySoldOut, result.FirstError);
        _loggerMock!.Verify(x => x.LogInfo("Tickets already sold out: 1."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_MultipleEventsNotSupported_ReturnsMultipleEventsNotSupportedError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 },
                new PurchaseTicketDetail { TicketId = 2 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1, 2 }))
            .ReturnsAsync(new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, SoldOut = false },
                new Ticket { Id = 2, EventId = 2, SoldOut = false }
            });

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.MultipleEventsNotSupported, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Tickets belong to multiple events, which is not supported."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1)).ReturnsAsync((Events?)null);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Event with ID 1 not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_EventCancelled_ReturnsEventCancelledAlreadyError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync(new Events { Id = 1, Status = EventStatus.Cancelled });

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.EventCancelledAlready, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Event with ID 1 has been cancelled."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_EventPast_ReturnsEventEndedAlreadyError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync(new Events { Id = 1, Status = EventStatus.Past });

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.EventEndedAlready, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Event with ID 1 has ended."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_BuyerNotFound_ReturnsUserNotFoundError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync(new Events { Id = 1, Capacity = 10, Status = EventStatus.Upcoming, OccupiedSeats = new List<int>() });
        _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns((User?)null);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Buyer with ID 1 not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_PaymentNotFound_ReturnsPaymentNotFoundError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync(new Events { Id = 1, Capacity = 10, Status = EventStatus.Upcoming, OccupiedSeats = new List<int>() });
        _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(new User { Id = 1 });
        _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(1)).Returns((Payment?)null);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Payment.PaymentNotFound, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Payment with ID 1 not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_InsufficientCapacity_ReturnsInsufficientCapacityError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 },
                new PurchaseTicketDetail { TicketId = 2 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1, 2 }))
            .ReturnsAsync(new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, SoldOut = false },
                new Ticket { Id = 2, EventId = 1, SoldOut = false }
            });
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync(new Events { Id = 1, Capacity = 1, Status = EventStatus.Upcoming, OccupiedSeats = new List<int>() });
        _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(new User { Id = 1 });
        _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(1)).Returns(new Payment { Id = 1 });

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.InsufficientCapacity, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Insufficient capacity for requested number of tickets."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoAvailableSeats_ReturnsNoAvailableSeatsError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 },
                new PurchaseTicketDetail { TicketId = 2 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1, 2 }))
            .ReturnsAsync(new List<Ticket>
            {
                new Ticket { Id = 1, EventId = 1, SoldOut = false },
                new Ticket { Id = 2, EventId = 1, SoldOut = false }
            });
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync(new Events { Id = 1, Capacity = 3, Status = EventStatus.Upcoming, OccupiedSeats = new List<int> { 1, 2 } });
        _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(new User { Id = 1 });
        _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(1)).Returns(new Payment { Id = 1 });

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.NoAvailableSeats, result.FirstError);
        _loggerMock!.Verify(x => x.LogError("Not enough available seats left."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ProcessingFailure_ReturnsFailureError()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1 }
            },
            PaymentId = 1
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } });
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync(new Events { Id = 1, Capacity = 10, Status = EventStatus.Upcoming, OccupiedSeats = new List<int>() });
        _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(new User { Id = 1 });
        _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(1)).Returns(new Payment { Id = 1 });
        _ticketRepositoryMock!.Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Ticket>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("Failed to process ticket purchase: Database error", result.FirstError.Description);
        _loggerMock!.Verify(x => x.LogError("Failed to process ticket purchase: Database error"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_FailureInAddEventTickets_ReturnsFailureErrorAndRollsBack()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
        {
            new PurchaseTicketDetail { TicketId = 1 }
        },
            PaymentId = 1
        };
        var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } };
        var eventDetails = new Events
        {
            Id = 1,
            Capacity = 10,
            OccupiedSeats = new List<int>(),
            Status = EventStatus.Upcoming
        };
        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(tickets);
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync(eventDetails);
        _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(new User { Id = 1 });
        _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(1)).Returns(new Payment { Id = 1 });
        _ticketRepositoryMock!.Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Ticket>>()))
            .Returns(Task.CompletedTask);
        _ticketRepositoryMock!.Setup(repo => repo.AddEventTicketsAsync(It.IsAny<List<EventTicket>>()))
            .ThrowsAsync(new Exception("Database error"));
        _eventRepositoryMock!.Setup(repo => repo.Update(It.IsAny<Events>()))
            .Verifiable();

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("Failed to process ticket purchase: Database error", result.FirstError.Description);
        Assert.IsTrue(tickets[0].SoldOut, "Ticket is marked as sold out in memory before transaction"); // Updated assertion
        Assert.AreEqual(10, eventDetails.Capacity, "Event capacity should not be updated in memory until Update is called");
        Assert.IsFalse(eventDetails.OccupiedSeats.Any(), "No seats should be occupied in memory until Update is called");
        _ticketRepositoryMock!.Verify(repo => repo.UpdateRangeAsync(It.IsAny<List<Ticket>>()), Times.Once());
        _ticketRepositoryMock!.Verify(repo => repo.AddEventTicketsAsync(It.IsAny<List<EventTicket>>()), Times.Once());
        _eventRepositoryMock!.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Never()); // Update not called due to exception
        _loggerMock!.Verify(x => x.LogError("Failed to process ticket purchase: Database error"), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ValidCommandWithSingleTicket_SuccessfullyProcessesTicket()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1, UserId = 1 }
            },
            PaymentId = 1
        };
        var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = 1, SoldOut = false } };
        var eventDetails = new Events
        {
            Id = 1,
            Capacity = 10,
            OccupiedSeats = new List<int>(),
            Status = EventStatus.Upcoming,
            EventTitle = "Test Event",
            StartDate = DateTime.Now.AddDays(1),
            UserId = 2
        };
        var buyer = new User { Id = 1, DisplayName = "Buyer" };
        var payment = new Payment { Id = 1 };

        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1 })).ReturnsAsync(tickets);
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1)).ReturnsAsync(eventDetails);
        _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(buyer);
        _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(1)).Returns(payment);
        _ticketRepositoryMock!.Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Ticket>>())).Returns(Task.CompletedTask);
        _ticketRepositoryMock!.Setup(repo => repo.AddEventTicketsAsync(It.IsAny<List<EventTicket>>())).Returns(Task.CompletedTask);
        _eventRepositoryMock!.Setup(repo => repo.Update(It.IsAny<Events>())).Verifiable();

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        var ticketResult = result.Value as CreateEventTicketResult;
        Assert.IsNotNull(ticketResult);
        Assert.AreEqual(1, ticketResult.EventTickets.Count);
        Assert.AreEqual(9, eventDetails.Capacity);
        Assert.IsTrue(eventDetails.OccupiedSeats.Contains(1));
        Assert.IsTrue(tickets[0].SoldOut);

        _ticketRepositoryMock!.Verify(repo => repo.UpdateRangeAsync(It.Is<List<Ticket>>(t => t.Count == 1)), Times.Once());
        _ticketRepositoryMock!.Verify(repo => repo.AddEventTicketsAsync(It.Is<List<EventTicket>>(et => et.Count == 1)), Times.Once());
        _eventRepositoryMock!.Verify(repo => repo.Update(It.Is<Events>(e => e.Capacity == 9)), Times.Once());
        _mediatorMock!.Verify(m => m.Publish(It.IsAny<TicketPurchasedEvent>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _loggerMock!.Verify(x => x.LogInfo("Assigned 1 tickets for event ID 1."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_ValidCommandWithMixedAssignments_SuccessfullyProcessesAndNotifies()
    {
        // Arrange
        var command = new AddEventTicketCommand
        {
            UserId = 1,
            PurchaseTicketDetail = new List<PurchaseTicketDetail>
            {
                new PurchaseTicketDetail { TicketId = 1, UserId = 1 }, // Buyer's ticket
                new PurchaseTicketDetail { TicketId = 2, UserId = 2 }, // Another user
                new PurchaseTicketDetail { TicketId = 3, Email = "guest@example.com" } // Guest
            },
            PaymentId = 1
        };
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, EventId = 1, SoldOut = false, TicketCategory = TicketCategory.Regular },
            new Ticket { Id = 2, EventId = 1, SoldOut = false, TicketCategory = TicketCategory.Vip },
            new Ticket { Id = 3, EventId = 1, SoldOut = false, TicketCategory = TicketCategory.VVip }
        };
        var eventDetails = new Events
        {
            Id = 1,
            Capacity = 10,
            OccupiedSeats = new List<int>(),
            Status = EventStatus.Upcoming,
            EventTitle = "Test Event",
            StartDate = DateTime.Now.AddDays(1),
            UserId = 3
        };
        var buyer = new User { Id = 1, DisplayName = "Buyer" };
        var payment = new Payment { Id = 1 };

        _ticketRepositoryMock!.Setup(repo => repo.GetTicketsByIdsAsync(new List<int> { 1, 2, 3 })).ReturnsAsync(tickets);
        _eventRepositoryMock!.Setup(repo => repo.GetEventByIdAsync(1)).ReturnsAsync(eventDetails);
        _userRepositoryMock!.Setup(repo => repo.GetUserById(1)).Returns(buyer);
        _paymentRepositoryMock!.Setup(repo => repo.GetPaymentById(1)).Returns(payment);
        _ticketRepositoryMock!.Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Ticket>>())).Returns(Task.CompletedTask);
        _ticketRepositoryMock!.Setup(repo => repo.AddEventTicketsAsync(It.IsAny<List<EventTicket>>())).Returns(Task.CompletedTask);
        _eventRepositoryMock!.Setup(repo => repo.Update(It.IsAny<Events>())).Verifiable();

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        var ticketResult = result.Value as CreateEventTicketResult;
        Assert.IsNotNull(ticketResult);
        Assert.AreEqual(3, ticketResult.EventTickets.Count);
        Assert.AreEqual(7, eventDetails.Capacity);
        Assert.IsTrue(eventDetails.OccupiedSeats.SequenceEqual(new List<int> { 1, 2, 3 }));
        Assert.IsTrue(tickets.All(t => t.SoldOut));

        _ticketRepositoryMock!.Verify(repo => repo.UpdateRangeAsync(It.Is<List<Ticket>>(t => t.Count == 3)), Times.Once());
        _ticketRepositoryMock!.Verify(repo => repo.AddEventTicketsAsync(It.Is<List<EventTicket>>(et => et.Count == 3)), Times.Once());
        _eventRepositoryMock!.Verify(repo => repo.Update(It.Is<Events>(e => e.Capacity == 7)), Times.Once());
        _mediatorMock!.Verify(m => m.Publish(It.IsAny<TicketPurchasedEvent>(), It.IsAny<CancellationToken>()), Times.Exactly(4)); // 1 for user, 1 for guest, 1 for buyer ticket, 1 for summary
        _loggerMock!.Verify(x => x.LogInfo("Assigned 3 tickets for event ID 1."), Times.Once());
    }
}