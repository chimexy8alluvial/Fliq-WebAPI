using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Application.Event.Commands.UpdateTicket
{
    public class UpdateTicketCommand : IRequest<ErrorOr<CreateTicketResult>>
    {
        public int EventId { get; set; }
        public int Id { get; set; }
        public string? TicketName { get; set; }
        public TicketType? TicketType { get; set; }
        public string? TicketDescription { get; set; }
        public DateTime? EventDate { get; set; }
        public decimal? Amount { get; set; }
        public string? MaximumLimit { get; set; }
        public bool? SoldOut { get; set; }
        public List<Discount>? Discounts { get; set; }
    }

    public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, ErrorOr<CreateTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly ITicketRepository _ticketRepository;

        public UpdateTicketCommandHandler(IEventRepository eventRepository, ILoggerManager logger, ITicketRepository ticketRepository)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _ticketRepository = ticketRepository;
        }

        public async Task<ErrorOr<CreateTicketResult>> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
        {
            // Validate event
            var eventEntity = _eventRepository.GetEventById(command.EventId);
            if (eventEntity == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            // Retrieve ticket
            var ticket = _ticketRepository.GetTicketById(command.Id);
            if (ticket == null || ticket.EventId != command.EventId)
            {
                _logger.LogError($"Ticket with ID {command.Id} for event ID {command.EventId} not found.");
                return Errors.Ticket.TicketNotFound;
            }

            // Update properties if provided
            var ticketToUpdtate = command.Adapt(ticket);

            // Update ticket and save changes
            _ticketRepository.Update(ticketToUpdtate);

            _logger.LogInfo($"Ticket '{command.TicketName ?? ticket.TicketName}' (ID {command.Id}) updated for event ID {command.EventId}.");
            return new CreateTicketResult(ticketToUpdtate);
        }
    }
}