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
        var eventTicketIds = new List<int> { 1, 2 };
        var command = new RefundTicketCommand { EventId = eventId, UserId = userId, EventTicketIds = eventTicketIds };

        var eventDetails = new Events { Id = eventId, Capacity = 10, OccupiedSeats = new List<int> { 1, 2 }, UserId = 3 };
        var user = new User { Id = userId, DisplayName = "Test User" };
        var tickets = eventTicketIds.Select(id => new EventTicket
        {
            Id = id,
            UserId = userId,
            SeatNumber = id,
            IsRefunded = false, // IsRefunded on EventTicket
            Ticket = new Ticket { EventId = eventId }
        }).ToList();

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _userRepositoryMock!.Setup(r => r.GetUserById(userId)).Returns(user);
        _ticketRepositoryMock!.Setup(r => r.GetEventTicketsByIds(eventTicketIds)).Returns(tickets);
        _mediatorMock!.Setup(m => m.Publish(It.IsAny<TicketRefundedEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(2, result.Value.RefundedTickets.Count);
        Assert.IsTrue(tickets.All(t => t.IsRefunded)); // Check IsRefunded on EventTicket
        Assert.AreEqual(12, eventDetails.Capacity);
        Assert.IsFalse(eventDetails.OccupiedSeats.Any());
        _loggerMock!.Verify(l => l.LogInfo($"Refunded {tickets.Count} tickets for EventId {eventId}."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_EventNotFound_ReturnsError()
    {
        // Arrange
        var command = new RefundTicketCommand { EventId = 1, UserId = 2, EventTicketIds = new List<int> { 1 } };
        _eventRepositoryMock!.Setup(r => r.GetEventById(1)).Returns((Events)null!);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Event with ID {command.EventId} not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_AlreadyRefunded_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        int userId = 2;
        var eventTicketIds = new List<int> { 1 };
        var command = new RefundTicketCommand { EventId = eventId, UserId = userId, EventTicketIds = eventTicketIds };

        var eventDetails = new Events { Id = eventId, Capacity = 10, OccupiedSeats = new List<int> { 1 } };
        var user = new User { Id = userId, DisplayName = "Test User" };
        var tickets = new List<EventTicket>
        {
            new EventTicket
            {
                Id = 1,
                UserId = userId,
                SeatNumber = 1,
                IsRefunded = true, // Already refunded
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