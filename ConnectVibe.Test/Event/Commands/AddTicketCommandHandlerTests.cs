using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Models;
using Fliq.Application.Event.Commands.Tickets;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Profile;
using MapsterMapper;
using Moq;

namespace Fliq.Test.Event.Commands
{
    [TestClass]
    public class AddTicketCommandHandlerTests
    {
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ITicketRepository>? _ticketRepositoryMock;
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IMapper>? _mapperMock;
        private Mock<ILocationService>? _locationServiceMock;
        private Mock<ICurrencyRepository>? _currencyRepositoryMock;

        private AddTicketCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _loggerMock = new Mock<ILoggerManager>();
            _mapperMock = new Mock<IMapper>();
            _locationServiceMock = new Mock<ILocationService>();
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();

            _handler = new AddTicketCommandHandler(
                _eventRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _ticketRepositoryMock.Object,
                _locationServiceMock.Object,
                _currencyRepositoryMock.Object
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
                TicketDescription = "VIP access",
                EventDate = DateTime.UtcNow,
                Amount = 100.0m,
                MaximumLimit = 50,
                SoldOut = false
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
            _loggerMock?.Verify(logger => logger.LogError($"Event with ID {command.EventId} not found."), Times.Once);
        }

        [TestMethod]
        public async Task Handle_EventHasNoLocation_ReturnsLocationNotFoundError()
        {
            // Arrange
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "VIP Ticket",
                TicketType = TicketType.Vip,
                TicketDescription = "VIP access",
                EventDate = DateTime.UtcNow,
                Amount = 100.0m,
                MaximumLimit = 50,
                SoldOut = false
            };

            var eventEntity = new Events
            {
                Id = 1,
                Tickets = new List<Ticket>(),
                Location = null // No location
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.LocationNotFound, result.FirstError);
            _loggerMock?.Verify(logger => logger.LogError($"Event with ID {command.EventId} has no location specified."), Times.Once);
        }

        [TestMethod]
        public async Task Handle_LocationServiceReturnsNullCurrency_ReturnsCurrencyNotFoundError()
        {
            // Arrange
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "VIP Ticket",
                TicketType = TicketType.Vip,
                TicketDescription = "VIP access",
                EventDate = DateTime.UtcNow,
                Amount = 100.0m,
                MaximumLimit = 50,
                SoldOut = false
            };

            var eventEntity = new Events
            {
                Id = 1,
                Tickets = new List<Ticket>(),
                Location = new Location { Lat = 40.7128, Lng = -74.0060 }
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);
            _locationServiceMock?.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync((LocationQueryResponse)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Payment.CurrencyNotFound, result.FirstError);
            _loggerMock?.Verify(logger => logger.LogError($"Unable to determine currency for location ({eventEntity.Location.Lat}, {eventEntity.Location.Lng})."), Times.Once);
        }

        [TestMethod]
        public async Task Handle_CurrencyNotSupported_ReturnsInvalidPayloadError()
        {
            // Arrange
            var command = new AddTicketCommand
            {
                EventId = 1,
                TicketName = "VIP Ticket",
                TicketType = TicketType.Vip,
                TicketDescription = "VIP access",
                EventDate = DateTime.UtcNow,
                Amount = 100.0m,
                MaximumLimit = 50,
                SoldOut = false
            };

            var eventEntity = new Events
            {
                Id = 1,
                Tickets = new List<Ticket>(),
                Location = new Location { Lat = 40.7128, Lng = -74.0060 }
            };

            var locationResponse = new LocationQueryResponse { CurrencyCode = "USD" };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);
            _locationServiceMock?.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(locationResponse);
            _currencyRepositoryMock?.Setup(repo => repo.GetCurrencyByCode(It.IsAny<string>())).Returns((Currency)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Payment.InvalidPayload, result.FirstError);
            _loggerMock?.Verify(logger => logger.LogError($"Currency code {locationResponse.CurrencyCode} not supported."), Times.Once);
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
                MaximumLimit = 100,
                SoldOut = false
            };

            var eventEntity = new Events
            {
                Id = 1,
                Tickets = new List<Ticket>(),
                Location = new Location { Lat = 40.7128, Lng = -74.0060 }
            };

            var currency = new Currency { Id = 1, CurrencyCode = "USD" };
            var locationResponse = new LocationQueryResponse { CurrencyCode = "USD" };

            var ticket = new Ticket
            {
                TicketName = command.TicketName,
                TicketType = command.TicketType,
                Amount = command.Amount,
                EventId = command.EventId,
                CurrencyId = currency.Id,
                Currency = currency
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);
            _locationServiceMock?.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(locationResponse);
            _currencyRepositoryMock?.Setup(repo => repo.GetCurrencyByCode(It.IsAny<string>())).Returns(currency);
            _mapperMock?.Setup(mapper => mapper.Map<Ticket>(command)).Returns(ticket);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(CreateTicketResult));
            Assert.AreEqual(ticket, result.Value.Ticket);
            _ticketRepositoryMock?.Verify(repo => repo.Add(It.Is<Ticket>(t => t.CurrencyId == currency.Id && t.EventId == eventEntity.Id)), Times.Once);
            _loggerMock?.Verify(logger => logger.LogInfo($"Ticket '{command.TicketName}' added to event ID {command.EventId} with currency {currency.CurrencyCode}."), Times.Once);
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
                MaximumLimit = 100,
                SoldOut = false,
                Discounts = new List<Discount>
                {
                    new Discount
                    {
                        Name = "5% off for booking in the next 7 days",
                        Percentage = 10.3,
                        NumberOfTickets = 4
                    }
                }
            };

            var eventEntity = new Events
            {
                Id = 1,
                Tickets = new List<Ticket>(),
                Location = new Location { Lat = 40.7128, Lng = -74.0060 }
            };

            var currency = new Currency { Id = 1, CurrencyCode = "USD" };
            var locationResponse = new LocationQueryResponse { CurrencyCode = "USD" };

            var ticket = new Ticket
            {
                TicketName = command.TicketName,
                TicketType = command.TicketType,
                Amount = command.Amount,
                Discounts = command.Discounts,
                EventId = command.EventId,
                CurrencyId = currency.Id,
                Currency = currency
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(eventEntity);
            _locationServiceMock?.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(locationResponse);
            _currencyRepositoryMock?.Setup(repo => repo.GetCurrencyByCode(It.IsAny<string>())).Returns(currency);
            _mapperMock?.Setup(mapper => mapper.Map<Ticket>(command)).Returns(ticket);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsInstanceOfType(result.Value, typeof(CreateTicketResult));
            Assert.AreEqual(command.Discounts.Count, result.Value.Ticket.Discounts?.Count);
            _ticketRepositoryMock?.Verify(repo => repo.Add(It.Is<Ticket>(t => t.Discounts.Count == command.Discounts.Count)), Times.Once);
            _loggerMock?.Verify(logger => logger.LogInfo($"Ticket '{command.TicketName}' added to event ID {command.EventId} with currency {currency.CurrencyCode}."), Times.Once);
        }
    }

  
}