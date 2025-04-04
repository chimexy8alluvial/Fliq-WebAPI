using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.StopTicketSales;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MediatR;
using Moq;

[TestClass]
public class StopTicketSalesCommandHandlerTests
{
    private Mock<IEventRepository>? _eventRepositoryMock;
    private Mock<ILoggerManager>? _loggerMock;
    private Mock<ITicketRepository>? _ticketRepositoryMock;
    private Mock<IMediator>? _mediatorMock;
    private StopTicketSalesCommandHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _loggerMock = new Mock<ILoggerManager>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _mediatorMock = new Mock<IMediator>();
        _handler = new StopTicketSalesCommandHandler(
            _eventRepositoryMock.Object,
            _loggerMock.Object,
            _ticketRepositoryMock.Object,
            _mediatorMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidStop_ReturnsUpdatedTicketCount()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand { EventId = eventId };

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            UserId = 2
        };
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, EventId = eventId, TicketName = "Ticket 1", SoldOut = false, Amount = 100m },
            new Ticket { Id = 2, EventId = eventId, TicketName = "Ticket 2", SoldOut = false, Amount = 150m }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByEventId(eventId)).Returns(tickets);
        _ticketRepositoryMock!.Setup(r => r.Update(It.IsAny<Ticket>())).Verifiable();

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        
        Assert.AreEqual(2, result.Value.UpdatedTicketCount);
        Assert.IsTrue(tickets.All(t => t.SoldOut));
        _ticketRepositoryMock!.Verify(r => r.Update(It.IsAny<Ticket>()), Times.Exactly(2));
        _loggerMock!.Verify(l => l.LogInfo($"Stopped ticket sales for EventId {eventId}. Updated 2 tickets."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_EventNotFound_ReturnsError()
    {
        // Arrange
        var command = new StopTicketSalesCommand { EventId = 1 };
        _eventRepositoryMock!.Setup(r => r.GetEventById(1)).Returns((Events)null!);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Event with ID {command.EventId} not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoTicketsFound_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand { EventId = eventId };

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            UserId = 2
        };
        var tickets = new List<Ticket>();

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByEventId(eventId)).Returns(tickets);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"No tickets found for EventId {eventId}."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_AllTicketsAlreadySoldOut_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand { EventId = eventId };

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            UserId = 2
        };
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, EventId = eventId, TicketName = "Ticket 1", SoldOut = true, Amount = 100m },
            new Ticket { Id = 2, EventId = eventId, TicketName = "Ticket 2", SoldOut = true, Amount = 150m }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByEventId(eventId)).Returns(tickets);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketAlreadySoldOut, result.FirstError);
        _loggerMock!.Verify(l => l.LogInfo($"All tickets for EventId {eventId} are already marked as sold out."), Times.Once());
    }
}