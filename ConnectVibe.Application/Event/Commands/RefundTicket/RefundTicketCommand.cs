using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.Event.Commands.RefundTicket
{
    public record RefundTicketCommand(int EventId, int UserId, List<int> EventTicketIds) : IRequest<ErrorOr<RefundTicketResult>>;

    public class RefundTicketCommandHandler : IRequestHandler<RefundTicketCommand, ErrorOr<RefundTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly ITicketRepository _ticketRepository;
        private readonly RefundTicketValidator _validator;
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
            _validator = new RefundTicketValidator(eventRepository, logger, ticketRepository, userRepository);
            _mediator = mediator;
        }

        public async Task<ErrorOr<RefundTicketResult>> Handle(RefundTicketCommand command, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationResult = await _validator.Validate(command);
            if (validationResult.IsError)
            {
                return validationResult.Errors;
            }

            var (eventDetails, user, ticketsToRefund) = validationResult.Value;

            // Process refund
            foreach (var eventTicket in ticketsToRefund)
            {
                eventTicket.IsRefunded = true;
                eventDetails.OccupiedSeats.Remove(eventTicket.SeatNumber);
                _ticketRepository.UpdateEventTicket(eventTicket);
            }

            eventDetails.Capacity += ticketsToRefund.Count;
            _eventRepository.Update(eventDetails);
            _logger.LogInfo($"Refunded {ticketsToRefund.Count} tickets for EventId {command.EventId}.");

            // Generate and send notifications
            var notifications = GenerateRefundNotifications(user, eventDetails, ticketsToRefund.Count);
            await _mediator.Publish(notifications.BuyerNotification, cancellationToken);
            await _mediator.Publish(notifications.OrganizerNotification, cancellationToken);

            return new RefundTicketResult(ticketsToRefund);
        }

        private (TicketRefundedEvent BuyerNotification, TicketRefundedEvent OrganizerNotification) GenerateRefundNotifications(
            User user, Events eventDetails, int ticketCount)
        {
            var ticketWord = ticketCount == 1 ? "ticket" : "tickets";

            // Buyer notification
            var buyerNotificationTitle = "Your Ticket Refund Confirmation";
            var buyerNotificationMessage = $"Dear {user.DisplayName}, your {ticketCount} {ticketWord} for '{eventDetails.EventTitle}' on {eventDetails.StartDate:MMMM dd, yyyy} have been successfully refunded.";

            var buyerNotification = new TicketRefundedEvent(
                user.Id, // BuyerId
                eventDetails.UserId, // OrganizerId
                eventDetails.Id,
                ticketCount,
                eventDetails.EventTitle,
                buyerNotificationTitle,
                buyerNotificationMessage,
                eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                user.DisplayName
            );

            // Organizer notification
            var organizerNotificationTitle = "Ticket Refund Notification";
            var organizerNotificationMessage = $"{user.DisplayName}'s {ticketCount} {ticketWord} for your event '{eventDetails.EventTitle}' on {eventDetails.StartDate:MMMM dd, yyyy} have been successfully refunded. The event capacity has been updated. Please check your event dashboard for details.";

            var organizerNotification = new TicketRefundedEvent(
                eventDetails.UserId, // OrganizerId (recipient)
                eventDetails.UserId, // OrganizerId
                eventDetails.Id,
                ticketCount,
                eventDetails.EventTitle,
                organizerNotificationTitle,
                organizerNotificationMessage,
                eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                user.DisplayName
            );

            return (buyerNotification, organizerNotification);
        }

        public class RefundTicketValidator
        {
            public Events EventDetails { get; set; }
            public User User { get; set; }
            public List<EventTicket> TicketsToRefund { get; set; }

            private readonly IEventRepository _eventRepository;
            private readonly ILoggerManager _logger;
            private readonly ITicketRepository _ticketRepository;
            private readonly IUserRepository _userRepository;

            public RefundTicketValidator(
               user,
                ILoggerManager logger,
                ITicketRepository ticketRepository,
                IUserRepository userRepository)
            {
                _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            }

            public async Task<ErrorOr<(Events Event, User User, List<EventTicket> Tickets)>> Validate(RefundTicketCommand command)
            {
                EventDetails = _eventRepository.GetEventById(command.EventId);
                if (EventDetails == null)
                {
                    _logger.LogError($"Event with ID {command.EventId} not found.");
                    return Errors.Event.EventNotFound;
                }

                User = _userRepository.GetUserById(command.UserId);
                if (User == null)
                {
                    _logger.LogError($"User with ID {command.UserId} not found.");
                    return Errors.User.UserNotFound;
                }

                TicketsToRefund = _ticketRepository.GetEventTicketsByIds(command.EventTicketIds)
                    .Where(t => t.UserId == command.UserId && t.Ticket.EventId == command.EventId)
                    .ToList();

                if (!TicketsToRefund.Any())
                {
                    _logger.LogError($"No valid tickets found for refund for UserId {command.UserId} and EventId {command.EventId}.");
                    return Errors.Ticket.TicketNotFound;
                }

                if (TicketsToRefund.Any(t => t.IsRefunded))
                {
                    _logger.LogInfo($"Some tickets for EventId {command.EventId} are already refunded.");
                    return Errors.Ticket.TicketAlreadyRefunded;
                }

                return (EventDetails, User, TicketsToRefund);
            }
        }
    }
}