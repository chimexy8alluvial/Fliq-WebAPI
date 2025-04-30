using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.Event.Commands.RefundTicket;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Infrastructure.Services.PaymentServices;
using MediatR;
using Moq;
using Moq.Protected;

[TestClass]
public class RefundTicketCommandHandlerTests
{
    private Mock<IEventRepository>? _eventRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<IUserRepository>? _userRepositoryMock;
    private Mock<IMediator>? _mediatorMock;
    private Mock<IPaymentRepository>? _paymentRepositoryMock;
    private Mock<IRevenueCatServices>? _revenueCatServicesMock;
    private RefundTicketCommandHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mediatorMock = new Mock<IMediator>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _revenueCatServicesMock = new Mock<IRevenueCatServices>();
        _handler = new RefundTicketCommandHandler(
            _eventRepositoryMock.Object,
            _loggerMock.Object,
            _ticketRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mediatorMock.Object,
            _paymentRepositoryMock.Object,
            _revenueCatServicesMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidRefund_ReturnsRefundedTickets()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        int organizerId = 3;
        int paymentId = 100;
        var eventTicketIds = new List<int> { 1, 2 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = new DateTime(2025, 4, 30),
            Capacity = 10,
            OccupiedSeats = new List<int> { 1, 2 },
            UserId = organizerId
        };
        var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
        var eventTickets = eventTicketIds.Select(id => new EventTicket
        {
            Id = id,
            UserId = userId,
            SeatNumber = id,
            IsRefunded = false,
            TicketId = id,
            PaymentId = paymentId,
            Ticket = new Ticket { Id = id, EventId = eventId, SoldOut = true }
        }).ToList();
        var tickets = eventTicketIds.Select(id => new Ticket
        {
            Id = id,
            EventId = eventId,
            SoldOut = true
        }).ToList();
        var payment = new Payment { Id = paymentId, TransactionId = "txn_100", Status = PaymentStatus.Success, UserId = userId};

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(eventTickets);
        _ticketRepositoryMock.Setup(r => r.GetTicketsByIds(eventTicketIds)).Returns(tickets);
        _ticketRepositoryMock.Setup(r => r.UpdateEventTicket(It.IsAny<EventTicket>()));
        _ticketRepositoryMock.Setup(r => r.Update(It.IsAny<Ticket>()));
        _eventRepositoryMock.Setup(r => r.Update(It.IsAny<Events>()));
        _paymentRepositoryMock!.Setup(r => r.GetPaymentById(paymentId)).Returns(payment);
        _paymentRepositoryMock.Setup(r => r.GetPaymentByTransactionId("txn_100")).Returns(payment);
        _paymentRepositoryMock.Setup(r => r.Update(It.IsAny<Payment>()));
        _mediatorMock!.Setup(m => m.Publish(It.IsAny<TicketRefundedEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Mock HttpClientFactory
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("txn_100")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        var httpClient = new HttpClient(httpMessageHandler.Object);
        httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var revenueCatServices = new RevenueCatServices(
            _paymentRepositoryMock.Object,
            _loggerMock!.Object,
            httpClientFactoryMock.Object
        );

        var handler = new RefundTicketCommandHandler(
            _eventRepositoryMock.Object,
            _loggerMock.Object,
            _ticketRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mediatorMock.Object,
            _paymentRepositoryMock.Object,
            revenueCatServices
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(2, result.Value.RefundedTickets.Count);
        Assert.IsTrue(eventTickets.All(t => t.IsRefunded));
        Assert.IsTrue(tickets.All(t => !t.SoldOut));
        Assert.AreEqual(12, eventDetails.Capacity);
        Assert.IsFalse(eventDetails.OccupiedSeats.Any());
        _ticketRepositoryMock.Verify(r => r.UpdateEventTicket(It.IsAny<EventTicket>()), Times.Exactly(2));
        _ticketRepositoryMock.Verify(r => r.Update(It.IsAny<Ticket>()), Times.Exactly(2));
        _eventRepositoryMock.Verify(r => r.Update(It.Is<Events>(e => e.Id == eventId && e.Capacity == 12 && !e.OccupiedSeats.Any())), Times.Once());
        _paymentRepositoryMock.Verify(r => r.Update(It.Is<Payment>(p => p.Status == PaymentStatus.Refunded && p.DateModified != null)), Times.Once());
        _loggerMock!.Verify(l => l.LogInfo($"Refunded {eventTickets.Count} tickets for EventId {eventId}."), Times.Once());
        _loggerMock!.Verify(l => l.LogInfo($"Successfully refunded transaction txn_100."), Times.Once());
        httpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );

        // Verify notifications
        _mediatorMock.Verify(m => m.Publish(
            It.Is<TicketRefundedEvent>(e =>
                e.BuyerId == userId &&
                e.OrganizerId == organizerId &&
                e.EventId == eventId &&
                e.NumberOfTickets == 2 &&
                e.EventTitle == "Test Event" &&
                e.Title == "Your Ticket Refund Confirmation" &&
                e.Message == "Dear Test User, your 2 tickets for 'Test Event' on April 30, 2025 have been successfully refunded." &&
                e.EventDate == "April 30, 2025" &&
                e.BuyerName == "Test User"),
            It.IsAny<CancellationToken>()), Times.Once());

        _mediatorMock.Verify(m => m.Publish(
            It.Is<TicketRefundedEvent>(e =>
                e.BuyerId == organizerId &&
                e.OrganizerId == organizerId &&
                e.EventId == eventId &&
                e.NumberOfTickets == 2 &&
                e.EventTitle == "Test Event" &&
                e.Title == "Ticket Refund Notification" &&
                e.Message == "Test User's 2 tickets for your event 'Test Event' on April 30, 2025 have been successfully refunded. The event capacity has been updated. Please check your event dashboard for details." &&
                e.EventDate == "April 30, 2025" &&
                e.BuyerName == "Test User"),
            It.IsAny<CancellationToken>()), Times.Once());
    }

    [TestMethod]
    public async Task Handle_PaymentNotFound_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        int paymentId = 100;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events { Id = eventId, UserId = 3 };
        var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
        var eventTickets = new List<EventTicket>
        {
            new EventTicket { Id = 1, UserId = userId, TicketId = 1, PaymentId = paymentId, Ticket = new Ticket { Id = 1, EventId = eventId } }
        };
        var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = eventId } };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(eventTickets);
        _ticketRepositoryMock.Setup(r => r.GetTicketsByIds(new List<int> { 1 })).Returns(tickets);
        _paymentRepositoryMock!.Setup(r => r.GetPaymentById(paymentId)).Returns((Payment)null!);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Payment.PaymentNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Payment with ID {paymentId} not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_InvalidTransactionId_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        int paymentId = 100;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events { Id = eventId, UserId = 3 };
        var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
        var eventTickets = new List<EventTicket>
        {
            new EventTicket { Id = 1, UserId = userId, TicketId = 1, PaymentId = paymentId, Ticket = new Ticket { Id = 1, EventId = eventId } }
        };
        var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = eventId } };
        var payment = new Payment { Id = paymentId, TransactionId = null!, Status = PaymentStatus.Success };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(eventTickets);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByIds(new List<int> { 1 })).Returns(tickets);
        _paymentRepositoryMock!.Setup(r => r.GetPaymentById(paymentId)).Returns(payment);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Payment.InvalidPaymentTransaction, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Payment ID {paymentId} has no associated transaction."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_RefundTransactionFails_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        int paymentId = 100;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events { Id = eventId, UserId = 3 };
        var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
        var eventTickets = new List<EventTicket>
        {
            new EventTicket { Id = 1, UserId = userId, TicketId = 1, PaymentId = paymentId, Ticket = new Ticket { Id = 1, EventId = eventId } }
        };
        var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = eventId } };
        var payment = new Payment { Id = paymentId, TransactionId = "txn_100", Status = PaymentStatus.Success };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(eventTickets);
        _ticketRepositoryMock.Setup(r => r.GetTicketsByIds(new List<int> { 1 })).Returns(tickets);
        _paymentRepositoryMock!.Setup(r => r.GetPaymentById(paymentId)).Returns(payment);
        _revenueCatServicesMock!.Setup(r => r.RefundTransactionAsync("txn_100"))
            .ReturnsAsync(Errors.Payment.FailedToProcess);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Payment.FailedToProcess, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Failed to refund transaction txn_100 for payment ID {paymentId}."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_AlreadyRefundedPayment_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        int paymentId = 100;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events { Id = eventId, UserId = 3 };
        var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
        var eventTickets = new List<EventTicket>
        {
            new EventTicket { Id = 1, UserId = userId, TicketId = 1, PaymentId = paymentId, Ticket = new Ticket { Id = 1, EventId = eventId } }
        };
        var tickets = new List<Ticket> { new Ticket { Id = 1, EventId = eventId } };
        var payment = new Payment { Id = paymentId, TransactionId = "txn_100", Status = PaymentStatus.Success };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(eventTickets);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByIds(new List<int> { 1 })).Returns(tickets);
        _paymentRepositoryMock!.Setup(r => r.GetPaymentById(paymentId)).Returns(payment);
        _revenueCatServicesMock!.Setup(r => r.RefundTransactionAsync("txn_100"))
            .ReturnsAsync(Error.Conflict(description: "Payment is already refunded."));

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Error.Conflict(description: "Payment is already refunded."), result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Failed to refund transaction txn_100 for payment ID {paymentId}."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_MissingTicketEntities_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        int paymentId = 100;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events { Id = eventId, UserId = 3 };
        var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
        var eventTickets = new List<EventTicket>
        {
            new EventTicket { Id = 1, UserId = userId, TicketId = 1, PaymentId = paymentId, Ticket = new Ticket { Id = 1, EventId = eventId } }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(eventTickets);
        _ticketRepositoryMock.Setup(r => r.GetTicketsByIds(new List<int> { 1 }))
            .Returns(new List<Ticket>());
        _paymentRepositoryMock!.Setup(r => r.GetPaymentById(paymentId)).Returns(new Payment { Id = paymentId, TransactionId = "txn_100", Status = PaymentStatus.Success });

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Some tickets not found for IDs: {string.Join(", ", new[] { 1 })}."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_EventNotFound_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns((Events)null!);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Event with ID {eventId} not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_UserNotFound_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = new DateTime(2025, 4, 30),
            Capacity = 10,
            OccupiedSeats = new List<int> { 1 },
            UserId = 3
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns((User)null!);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"User with ID {userId} not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoValidTicketsFound_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = new DateTime(2025, 4, 30),
            Capacity = 10,
            OccupiedSeats = new List<int> { 1 },
            UserId = 3
        };
        var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
        var eventTickets = new List<EventTicket>
        {
            new EventTicket
            {
                Id = 1,
                UserId = 999,
                SeatNumber = 1,
                IsRefunded = false,
                TicketId = 1,
                PaymentId = 100,
                Ticket = new Ticket { Id = 1, EventId = eventId }
            }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(eventTickets);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"No valid tickets found for refund for UserId {userId} and EventId {eventId}."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_AlreadyRefunded_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand(eventId, userId, eventTicketIds);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = new DateTime(2025, 4, 30),
            Capacity = 10,
            OccupiedSeats = new List<int> { 1 },
            UserId = 3
        };
        var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
        var eventTickets = new List<EventTicket>
        {
            new EventTicket
            {
                Id = 1,
                UserId = userId,
                SeatNumber = 1,
                IsRefunded = true,
                TicketId = 1,
                PaymentId = 100,
                Ticket = new Ticket { Id = 1, EventId = eventId }
            }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(eventTickets);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketAlreadyRefunded, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Some tickets for EventId {eventId} are already refunded."), Times.Once());
    }
}