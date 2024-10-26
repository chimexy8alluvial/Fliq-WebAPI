using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
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
        public string Currency { get; set; } = default!;
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

        public AddTicketCommandHandler(IEventRepository eventRepository, ILoggerManager logger, IMapper mapper, ITicketRepository ticketRepository)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _mapper = mapper;
            _ticketRepository = ticketRepository;
        }

        public async Task<ErrorOr<CreateTicketResult>> Handle(AddTicketCommand command, CancellationToken cancellationToken)
        {
            var eventEntity = _eventRepository.GetEventById(command.EventId);
            if (eventEntity == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            var newTicket = _mapper.Map<Ticket>(command);

            // Add the ticket to the event's Tickets list and update

            _ticketRepository.Add(newTicket);

            _logger.LogInfo($"Ticket '{command.TicketName}' added to event ID {command.EventId}.");
            return new CreateTicketResult(newTicket);
        }
    }
}