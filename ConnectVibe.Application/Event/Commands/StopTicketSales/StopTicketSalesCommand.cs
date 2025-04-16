using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Event.Commands.StopTicketSales
{
    public class StopTicketSalesCommand : IRequest<ErrorOr<StopTicketSalesResult>>
    {
        public int EventId { get; set; }
    }

    public class StopTicketSalesCommandHandler : IRequestHandler<StopTicketSalesCommand, ErrorOr<StopTicketSalesResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly ITicketRepository _ticketRepository;
        private readonly IMediator _mediator;

        public StopTicketSalesCommandHandler(
            IEventRepository eventRepository,
            ILoggerManager logger,
            ITicketRepository ticketRepository,
            IMediator mediator)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _ticketRepository = ticketRepository;
            _mediator = mediator;
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

            // Check if all tickets are already sold out
            if (tickets.All(t => t.SoldOut))
            {
                _logger.LogInfo($"All tickets for EventId {command.EventId} are already marked as sold out.");
                return Errors.Ticket.TicketAlreadySoldOut;
            }

            // Mark all tickets as SoldOut
            foreach (var ticket in tickets)
            {
                if (!ticket.SoldOut)
                {
                    ticket.SoldOut = true;
                    _ticketRepository.Update(ticket);
                }
            }

            _logger.LogInfo($"Stopped ticket sales for EventId {command.EventId}. Updated {tickets.Count(t => t.SoldOut)} tickets.");
           

            return new StopTicketSalesResult(tickets.Count);
        }
    }

    public record StopTicketSalesResult(int UpdatedTicketCount);
}