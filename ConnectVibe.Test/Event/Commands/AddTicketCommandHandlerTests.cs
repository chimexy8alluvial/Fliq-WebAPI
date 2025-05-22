using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.Tickets;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Fliq.Test.Event.Commands
{
    [TestClass]
    public class AddTicketCommandHandlerTests
    {
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ITicketRepository>? _ticketRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IMapper>? _mapperMock;
        private Mock<IProfileRepository>? _profileRepositoryMock;
        private Mock<IHttpContextAccessor>? _contextAccessorMock;

        private AddTicketCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _mapperMock = new Mock<IMapper>();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _contextAccessorMock = new Mock<IHttpContextAccessor>();

            // Setup default user ID
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));
            _contextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

            // Setup default currency
            _profileRepositoryMock.Setup(repo => repo.GetUserCurrencyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Currency { Id = 1, CurrencyCode = "USD" });

            _handler = new AddTicketCommandHandler(
                _eventRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _ticketRepositoryMock.Object,
                _profileRepositoryMock.Object,
                _contextAccessorMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "VIP Ticket",
                TicketType = TicketType.Vip,
                Amount = 100.0m
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_AddsTicketSuccessfully()
        {
            // Arrange
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "General Admission",
                TicketType = TicketType.Regular,
                TicketDescription = "Standard entry ticket",
                EventDate = DateTime.UtcNow,
                Amount = 50.0m,
                MaximumLimit = "100",
                SoldOut = false
            };

            var eventEntity = new Events
            {
                Id = 1,
                Tickets = new List<Ticket>()
            };

            var ticket = new Ticket
            {
                TicketName = command.TicketName,
                TicketType = command.TicketType,
                TicketDescription = command.TicketDescription,
                EventDate = command.EventDate,
                Amount = command.Amount,
                MaximumLimit = int.Parse(command.MaximumLimit),
                SoldOut = command.SoldOut,
                CurrencyId = 1,
                Currency = new Currency { Id = 1, CurrencyCode = "USD" }
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);
            _mapperMock?.Setup(mapper => mapper.Map<Ticket>(command)).Returns(ticket);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _ticketRepositoryMock?.Verify(repo => repo.Add(It.Is<Ticket>(t =>
                t.TicketName == command.TicketName &&
                t.TicketType == command.TicketType &&
                t.Amount == command.Amount &&
                t.CurrencyId == 1)), Times.Once());
            _loggerMock?.Verify(logger => logger.LogInfo(It.Is<string>(s =>
                s.Contains(command.TicketName) && s.Contains(command.EventId.ToString()))), Times.Once());
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(ticket.TicketName, result.Value.TicketName);
        }

        [TestMethod]
        public async Task Handle_DiscountsIncluded_AddsDiscountsToTicket()
        {
            // Arrange
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "Early Bird",
                TicketType = TicketType.Regular,
                TicketDescription = "Discounted ticket for early buyers",
                EventDate = DateTime.UtcNow,
                Amount = 30.0m,
                MaximumLimit = "50",
                SoldOut = false,
                Discounts = new List<Discount>
                {
                    new Discount
                    {
                        Name = "5% off for booking in the next 7 days",
                        Percentage = (double)10.3m,
                        NumberOfTickets = 4,
                        Type = (Domain.Entities.Event.Enums.DiscountType)1
                    }
                }
            };

            var eventEntity = new Events { Id = 1, Tickets = new List<Ticket>() };
            var ticket = new Ticket
            {
                TicketName = command.TicketName,
                TicketType = command.TicketType,
                TicketDescription = command.TicketDescription,
                EventDate = command.EventDate,
                Amount = command.Amount,
                MaximumLimit = int.Parse(command.MaximumLimit),
                SoldOut = command.SoldOut,
                Discounts = command.Discounts,
                CurrencyId = 1,
                Currency = new Currency { Id = 1, CurrencyCode = "USD" }
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);
            _mapperMock?.Setup(mapper => mapper.Map<Ticket>(command)).Returns(ticket);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.IsNotNull(result.Value.Discounts);
            Assert.AreEqual(command.Discounts.Count, result.Value.Discounts?.Count);
            _ticketRepositoryMock?.Verify(repo => repo.Add(It.Is<Ticket>(t =>
                t.TicketName == command.TicketName &&
                t.Discounts != null &&
                t.Discounts.Count == command.Discounts.Count &&
                t.Discounts[0].Name == command.Discounts[0].Name)), Times.Once());
            _loggerMock?.Verify(logger => logger.LogInfo(It.Is<string>(s =>
                s.Contains(command.TicketName) && s.Contains(command.EventId.ToString()))), Times.Once());
        }
    }
}