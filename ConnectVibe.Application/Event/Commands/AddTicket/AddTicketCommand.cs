using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Event.Commands.Tickets
{
    public class AddTicketCommand : IRequest<ErrorOr<CreateTicketResult>>
    {
        public int EventId { get; set; }
        public string TicketName { get; set; } = default!;
        public TicketType TicketType { get; set; } = default!;
        public string TicketDescription { get; set; } = default!;
        public DateTime EventDate { get; set; }
        public decimal Amount { get; set; } = default!;
        public int MaximumLimit { get; set; } = default!;
        public bool SoldOut { get; set; } = default!;
        public List<Discount>? Discounts { get; set; } = default!;
    }

    public class AddTicketCommandHandler : IRequestHandler<AddTicketCommand, ErrorOr<CreateTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ITicketRepository _ticketRepository;
        private readonly ILocationService _locationService;
        private readonly ICurrencyRepository _currencyRepository; 

        public AddTicketCommandHandler(
            IEventRepository eventRepository,
            ILoggerManager logger,
            IMapper mapper,
            ITicketRepository ticketRepository,
            ILocationService locationService,
            ICurrencyRepository currencyRepository)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _mapper = mapper;
            _ticketRepository = ticketRepository;
            _locationService = locationService;
            _currencyRepository = currencyRepository;
        }

        public async Task<ErrorOr<CreateTicketResult>> Handle(AddTicketCommand command, CancellationToken cancellationToken)
        {
            var eventEntity = _eventRepository.GetEventById(command.EventId);
            if (eventEntity == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            // Check for duplicate TicketType for this EventId
            var existingTicket = await _ticketRepository.GetTicketByEventIdAndTypeAsync(command.EventId, command.TicketType);
            if (existingTicket != null)
            {
                _logger.LogError($"A ticket with TicketType {command.TicketType} already exists for Event ID {command.EventId}.");
                return Errors.Ticket.DuplicateTicketType;
            }

            // Check if event has a location
            if (eventEntity.Location == null)
            {
                _logger.LogError($"Event with ID {command.EventId} has no location specified.");
                return Errors.Event.LocationNotFound;
            }

            // Get the currency code from the event's location
            var locationResponse = await _locationService.GetAddressFromCoordinatesAsync(
                eventEntity.Location.Lat,
                eventEntity.Location.Lng);
            if (locationResponse == null || string.IsNullOrEmpty(locationResponse.CurrencyCode))
            {
                _logger.LogError($"Unable to determine currency for location ({eventEntity.Location.Lat}, {eventEntity.Location.Lng}).");
                return Errors.Payment.CurrencyNotFound;
            }

            // Map currency code to Currency entity
            var currency = _currencyRepository.GetCurrencyByCode(locationResponse.CurrencyCode);
            if (currency == null)
            {
                _logger.LogError($"Currency code {locationResponse.CurrencyCode} not supported.");
                return Errors.Payment.InvalidPayload;
            }

            var newTicket = _mapper.Map<Ticket>(command);
            newTicket.CurrencyId = currency.Id;
            newTicket.Currency = currency; 
            newTicket.EventId = eventEntity.Id;

            _ticketRepository.Add(newTicket);

            _logger.LogInfo($"Ticket '{command.TicketName}' added to event ID {command.EventId} with currency {currency.CurrencyCode}.");
            return new CreateTicketResult(newTicket);
        }
    }
}