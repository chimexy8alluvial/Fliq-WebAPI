using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.RefundTicket;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using MediatR;
using Moq;

[TestClass]
public class RefundTicketCommandHandlerTests
{
    private Mock<IEventRepository>? _eventRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<IUserRepository>? _userRepositoryMock;
    private Mock<IMediator>? _mediatorMock;
    private RefundTicketCommandHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mediatorMock = new Mock<IMediator>();
        _handler = new RefundTicketCommandHandler(
            _eventRepositoryMock.Object,
            _loggerMock.Object,
            _ticketRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mediatorMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidRefund_ReturnsRefundedTickets()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        int organizerId = 3;
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
        var user = new User { Id = userId, DisplayName = "Test User" };
        var tickets = eventTicketIds.Select(id => new EventTicket
        {
            Id = id,
            UserId = userId,
            SeatNumber = id,
            IsRefunded = false,
            Ticket = new Ticket { EventId = eventId }
        }).ToList();

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(tickets);
        _ticketRepositoryMock!.Setup(r => r.UpdateEventTicket(It.IsAny<EventTicket>())).Verifiable();
        _eventRepositoryMock!.Setup(r => r.Update(It.IsAny<Events>())).Verifiable();
        _mediatorMock!.Setup(m => m.Publish(It.IsAny<TicketRefundedEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(2, result.Value.RefundedTickets.Count);
        Assert.IsTrue(tickets.All(t => t.IsRefunded));
        Assert.AreEqual(12, eventDetails.Capacity);
        Assert.IsFalse(eventDetails.OccupiedSeats.Any());
        _ticketRepositoryMock!.Verify(r => r.UpdateEventTicket(It.IsAny<EventTicket>()), Times.Exactly(2));
        _eventRepositoryMock!.Verify(r => r.Update(It.Is<Events>(e => e.Id == eventId && e.Capacity == 12 && !e.OccupiedSeats.Any())), Times.Once());
        _loggerMock!.Verify(l => l.LogInfo($"Refunded {tickets.Count} tickets for EventId {eventId}."), Times.Once());

        // Verify buyer notification
        _mediatorMock!.Verify(m => m.Publish(
            It.Is<TicketRefundedEvent>(e =>
                e.BuyerId == userId &&
                e.OrganizerId == organizerId &&
                e.EventId == eventId &&
                e.NumberOfTickets == 2 &&
                e.EventTitle == "Test Event" &&
                e.Title == "Your Ticket Refund Confirmation" &&
                e.Message == $"Dear Test User, your refund request for 2 ticket(s) to 'Test Event' on April 30, 2025 has been successfully processed." &&
                e.EventDate == "April 30, 2025" &&
                e.BuyerName == "Test User"),
            It.IsAny<CancellationToken>()), Times.Once());

        // Verify organizer notification
        _mediatorMock!.Verify(m => m.Publish(
            It.Is<TicketRefundedEvent>(e =>
                e.BuyerId == organizerId &&
                e.OrganizerId == organizerId &&
                e.EventId == eventId &&
                e.NumberOfTickets == 2 &&
                e.EventTitle == "Test Event" &&
                e.Title == "Ticket Refund Notification" &&
                e.Message == $"User Test User has refunded 2 ticket(s) for your event 'Test Event' on April 30, 2025. The event capacity has been updated." &&
                e.EventDate == "April 30, 2025" &&
                e.BuyerName == "Test User"),
            It.IsAny<CancellationToken>()), Times.Once());
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
        var user = new User { Id = userId, DisplayName = "Test User" };
        var tickets = new List<EventTicket>
        {
            new EventTicket
            {
                Id = 1,
                UserId = 999, // Different UserId
                SeatNumber = 1,
                IsRefunded = false,
                Ticket = new Ticket { EventId = eventId }
            }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(tickets);

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
        var user = new User { Id = userId, DisplayName = "Test User" };
        var tickets = new List<EventTicket>
        {
            new EventTicket
            {
                Id = 1,
                UserId = userId,
                SeatNumber = 1,
                IsRefunded = true,
                Ticket = new Ticket { EventId = eventId }
            }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(tickets);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketAlreadyRefunded, result.FirstError);
        _loggerMock!.Verify(l => l.LogInfo($"Some tickets for EventId {eventId} are already refunded."), Times.Once());
    }
}