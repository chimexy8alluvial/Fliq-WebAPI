using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
        public string MaximumLimit { get; set; } = default!;
        public bool SoldOut { get; set; } = default!;
        public List<Discount>? Discounts { get; set; } = default!;
    }

    public class AddTicketCommandHandler : IRequestHandler<AddTicketCommand, ErrorOr<CreateTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ITicketRepository _ticketRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public AddTicketCommandHandler(IEventRepository eventRepository, ILoggerManager logger, IMapper mapper, ITicketRepository ticketRepository, IProfileRepository profileRepository, IHttpContextAccessor contextAccessor)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _mapper = mapper;
            _ticketRepository = ticketRepository;
            _profileRepository = profileRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<ErrorOr<CreateTicketResult>> Handle(AddTicketCommand command, CancellationToken cancellationToken)
        {
            //var eventEntity = _eventRepository.GetEventById(command.EventId);
            //if (eventEntity == null)
            //{
            //    _logger.LogError($"Event with ID {command.EventId} not found.");
            //    return Errors.Event.EventNotFound;
            //}

            //var newTicket = _mapper.Map<Ticket>(command);

            //// Add the ticket to the event's Tickets list and update

            //_ticketRepository.Add(newTicket);

            //_logger.LogInfo($"Ticket '{command.TicketName}' added to event ID {command.EventId}.");
            //return new CreateTicketResult(newTicket);

            // Validate event
            var eventEntity = _eventRepository.GetEventById(command.EventId);
            if (eventEntity == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            // Get user ID
            var userId = GetAuthUserId();

            // Get user's currency
            Currency currency;
            try
            {
                currency = await _profileRepository.GetUserCurrencyAsync(userId, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Failed to retrieve currency for user ID {userId}: {ex.Message}");
                return Error.Failure("No currency available for ticket creation.");
            }

            // Map command to Ticket and set CurrencyId
            var newTicket = _mapper.Map<Ticket>(command);
            newTicket.CurrencyId = currency.Id;
            newTicket.Currency = currency;

            // Add ticket
            _ticketRepository.Add(newTicket);

            _logger.LogInfo($"Ticket '{command.TicketName}' added to event ID {command.EventId} with CurrencyId {currency.Id}.");
            return new CreateTicketResult(newTicket);
        }

        private int GetAuthUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogError("Failed to retrieve authenticated user ID.");
                throw new UnauthorizedAccessException("User not authenticated.");
            }
            return userId;
        }
    }
}