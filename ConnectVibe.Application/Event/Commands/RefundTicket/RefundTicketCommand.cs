using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.Event.Commands.RefundTicket
{
    public class RefundTicketCommand : IRequest<ErrorOr<RefundTicketResult>>
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public List<int> EventTicketIds { get; set; } = new List<int>();
    }

    public class RefundTicketCommandHandler : IRequestHandler<RefundTicketCommand, ErrorOr<RefundTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public RefundTicketCommandHandler(
            IEventRepository eventRepository,
            ILoggerManager logger,
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            IMediator mediator)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<RefundTicketResult>> Handle(RefundTicketCommand command, CancellationToken cancellationToken)
        {
            var eventDetails = _eventRepository.GetEventById(command.EventId);
            if (eventDetails == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                _logger.LogError($"User with ID {command.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            var ticketsToRefund = _ticketRepository.GetEventTicketsByIds(command.EventTicketIds)
                .Where(t => t.UserId == command.UserId && t.Ticket.EventId == command.EventId)
                .ToList();

            if (!ticketsToRefund.Any())
            {
                _logger.LogError($"No valid tickets found for refund for UserId {command.UserId} and EventId {command.EventId}.");
                return Errors.Ticket.TicketNotFound;
            }

            if (ticketsToRefund.Any(t => t.IsRefunded))
            {
                _logger.LogInfo($"Some tickets for EventId {command.EventId} are already refunded.");
                return Errors.Ticket.TicketAlreadyRefunded;
            }

            // Process refund
            foreach (var eventTicket in ticketsToRefund)
            {
                eventTicket.IsRefunded = true; // Update IsRefunded on EventTicket, not Ticket
                eventDetails.OccupiedSeats.Remove(eventTicket.SeatNumber);
                _ticketRepository.UpdateEventTicket(eventTicket);
            }

            eventDetails.Capacity += ticketsToRefund.Count;
            _eventRepository.Update(eventDetails);
            _logger.LogInfo($"Refunded {ticketsToRefund.Count} tickets for EventId {command.EventId}.");

            // Prepare notification details
            var notificationTitle = "Tickets Refunded";
            var notificationMessage = $"You have successfully refunded {ticketsToRefund.Count} ticket(s) for the event '{eventDetails.EventTitle}' on {eventDetails.StartDate}.";

            await _mediator.Publish(new TicketRefundedEvent(
                command.UserId,
                eventDetails.UserId, // Organizer ID
                eventDetails.Id,
                ticketsToRefund.Count,
                eventDetails.EventTitle,
                notificationTitle,
                notificationMessage,
                eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                user.DisplayName
            ), cancellationToken);

            return new RefundTicketResult(ticketsToRefund);
        }
    }

    public record RefundTicketResult(List<EventTicket> RefundedTickets);
}