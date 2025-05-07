using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.StopTicketSales;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
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
            _ticketRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidStop_ReturnsAffectedTicketCount()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand(eventId);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            UserId = 2,
            TicketSales = TicketSales.Active
        };
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, EventId = eventId, TicketName = "Ticket 1", SoldOut = false, Amount = 100m },
            new Ticket { Id = 2, EventId = eventId, TicketName = "Ticket 2", SoldOut = false, Amount = 150m }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByEventId(eventId)).Returns(tickets);
        _eventRepositoryMock!.Setup(r => r.Update(It.IsAny<Events>())).Verifiable();

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(2, result.Value.AffectedTicketCount);
        Assert.AreEqual(TicketSales.Inactive, eventDetails.TicketSales);
        _eventRepositoryMock!.Verify(r => r.Update(It.Is<Events>(e => e.Id == eventId && e.TicketSales == TicketSales.Inactive)), Times.Once());
        _ticketRepositoryMock!.Verify(r => r.Update(It.IsAny<Ticket>()), Times.Never());
        _loggerMock!.Verify(l => l.LogInfo($"Stopped ticket sales for EventId {eventId}. Affected 2 unsold tickets."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_MixedSoldAndUnsoldTickets_ReturnsCorrectAffectedCount()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand(eventId);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            UserId = 2,
            TicketSales = TicketSales.Active
        };
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, EventId = eventId, TicketName = "Ticket 1", SoldOut = true, Amount = 100m },
            new Ticket { Id = 2, EventId = eventId, TicketName = "Ticket 2", SoldOut = false, Amount = 150m },
            new Ticket { Id = 3, EventId = eventId, TicketName = "Ticket 3", SoldOut = false, Amount = 200m }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByEventId(eventId)).Returns(tickets);
        _eventRepositoryMock!.Setup(r => r.Update(It.IsAny<Events>())).Verifiable();

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(2, result.Value.AffectedTicketCount); // Only 2 unsold tickets
        Assert.AreEqual(TicketSales.Inactive, eventDetails.TicketSales);
        _eventRepositoryMock!.Verify(r => r.Update(It.Is<Events>(e => e.Id == eventId && e.TicketSales == TicketSales.Inactive)), Times.Once());
        _ticketRepositoryMock!.Verify(r => r.Update(It.IsAny<Ticket>()), Times.Never());
        _loggerMock!.Verify(l => l.LogInfo($"Stopped ticket sales for EventId {eventId}. Affected 2 unsold tickets."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_AllTicketsSoldOut_ReturnsAffectedTicketCount()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand(eventId);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            UserId = 2,
            TicketSales = TicketSales.Active
        };
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, EventId = eventId, TicketName = "Ticket 1", SoldOut = true, Amount = 100m },
            new Ticket { Id = 2, EventId = eventId, TicketName = "Ticket 2", SoldOut = true, Amount = 150m }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByEventId(eventId)).Returns(tickets);
        _eventRepositoryMock!.Setup(r => r.Update(It.IsAny<Events>())).Verifiable();

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(0, result.Value.AffectedTicketCount); // No unsold tickets
        Assert.AreEqual(TicketSales.Inactive, eventDetails.TicketSales);
        _eventRepositoryMock!.Verify(r => r.Update(It.Is<Events>(e => e.Id == eventId && e.TicketSales == TicketSales.Inactive)), Times.Once());
        _ticketRepositoryMock!.Verify(r => r.Update(It.IsAny<Ticket>()), Times.Never());
        _loggerMock!.Verify(l => l.LogInfo($"Stopped ticket sales for EventId {eventId}. Affected 0 unsold tickets."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_EventNotFound_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand(eventId);

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns((Events)null!);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        _loggerMock!.Verify(l => l.LogError($"Event with ID {eventId} not found."), Times.Once());
    }

    [TestMethod]
    public async Task Handle_NoTicketsFound_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand(eventId);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            UserId = 2,
            TicketSales = TicketSales.Active
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
    public async Task Handle_TicketSalesAlreadyStopped_ReturnsError()
    {
        // Arrange
        int eventId = 1;
        var command = new StopTicketSalesCommand(eventId);

        var eventDetails = new Events
        {
            Id = eventId,
            EventTitle = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            UserId = 2,
            TicketSales = TicketSales.Inactive
        };
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, EventId = eventId, TicketName = "Ticket 1", SoldOut = false, Amount = 100m },
            new Ticket { Id = 2, EventId = eventId, TicketName = "Ticket 2", SoldOut = true, Amount = 150m }
        };

        _eventRepositoryMock!.Setup(r => r.GetEventById(eventId)).Returns(eventDetails);
        _ticketRepositoryMock!.Setup(r => r.GetTicketsByEventId(eventId)).Returns(tickets);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(Errors.Ticket.TicketSalesAlreadyStopped, result.FirstError);
        _loggerMock!.Verify(l => l.LogInfo($"Ticket sales for EventId {eventId} are already stopped."), Times.Once());
    }
}