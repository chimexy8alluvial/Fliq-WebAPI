using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;

namespace Fliq.Application.Event.Commands.StopTicketSales
{
    public record StopTicketSalesCommand(int EventId) : IRequest<ErrorOr<StopTicketSalesResult>>;
   

    public class StopTicketSalesCommandHandler : IRequestHandler<StopTicketSalesCommand, ErrorOr<StopTicketSalesResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly ITicketRepository _ticketRepository;

        public StopTicketSalesCommandHandler(
            IEventRepository eventRepository,
            ILoggerManager logger,
            ITicketRepository ticketRepository)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _ticketRepository = ticketRepository;
        }

        public async Task<ErrorOr<StopTicketSalesResult>> Handle(StopTicketSalesCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var eventDetails = _eventRepository.GetEventById(command.EventId);
            if (eventDetails == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            // Fetch all tickets for the event
            var tickets = _ticketRepository.GetTicketsByEventId(command.EventId);
            if (!tickets.Any())
            {
                _logger.LogError($"No tickets found for EventId {command.EventId}.");
                return Errors.Ticket.TicketNotFound;
            }

            // Check if ticket sales are already stopped
            if (eventDetails.TicketSales == TicketSales.Inactive)
            {
                _logger.LogInfo($"Ticket sales for EventId {command.EventId} are already stopped.");
                return Errors.Ticket.TicketSalesAlreadyStopped;
            }

            // Count tickets that are not sold out (affected by stopping sales)
            int affectedTicketCount = tickets.Count(t => !t.SoldOut);

            // Stop ticket sales
            eventDetails.TicketSales = TicketSales.Inactive;
            _eventRepository.Update(eventDetails);

            _logger.LogInfo($"Stopped ticket sales for EventId {command.EventId}. Affected {affectedTicketCount} unsold tickets.");

            return new StopTicketSalesResult(affectedTicketCount);
        }
    }

    public record StopTicketSalesResult(int AffectedTicketCount);
}