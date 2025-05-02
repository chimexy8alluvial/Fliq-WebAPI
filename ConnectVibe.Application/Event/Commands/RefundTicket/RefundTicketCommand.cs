using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using MediatR;
using System.Transactions;

namespace Fliq.Application.Event.Commands.RefundTicket
{
    public record RefundTicketCommand(int EventId, int UserId, List<int> EventTicketIds) : IRequest<ErrorOr<RefundTicketResult>>;

    public class RefundTicketCommandHandler : IRequestHandler<RefundTicketCommand, ErrorOr<RefundTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IRevenueCatServices _revenueCatServices;

        public RefundTicketCommandHandler(
            IEventRepository eventRepository,
            ILoggerManager logger,
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            IMediator mediator,
            IPaymentRepository paymentRepository,
            IRevenueCatServices revenueCatServices)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _mediator = mediator;
            _paymentRepository = paymentRepository;
            _revenueCatServices = revenueCatServices;
        }

        public async Task<ErrorOr<RefundTicketResult>> Handle(RefundTicketCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await Validate(command);
            if (validationResult.IsError)
            {
                return validationResult.Errors;
            }
            var (validatedEventDetails, validatedUser, validatedTickets) = validationResult.Value;

            var refundResult = await ProcessTicketRefund(validatedEventDetails, validatedTickets);
            if (refundResult.IsError)
            {
                return refundResult.Errors;
            }

            var notifications = GenerateRefundNotifications(validatedUser, validatedEventDetails, validatedTickets.Count);
            await _mediator.Publish(notifications.BuyerNotification, cancellationToken);
            await _mediator.Publish(notifications.OrganizerNotification, cancellationToken);

            return new RefundTicketResult(validatedTickets);
        }

        private async Task<ErrorOr<(Events Event, User User, List<EventTicket> Tickets)>> Validate(RefundTicketCommand command)
        {
            var eventDetails =  _eventRepository.GetEventById(command.EventId);
            if (eventDetails == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            var user =  _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                _logger.LogError($"User with ID {command.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            var eventTicketsToRefund = (_ticketRepository.GetEventTicketsByIds(command.EventTicketIds))
                .Where(t => t.UserId == command.UserId && t.Ticket.EventId == command.EventId)
                .ToList();
            if (!eventTicketsToRefund.Any())
            {
                _logger.LogError($"No valid tickets found for refund for UserId {command.UserId} and EventId {command.EventId}.");
                return Errors.Ticket.TicketNotFound;
            }

            if (eventTicketsToRefund.Any(t => t.IsRefunded))
            {
                _logger.LogError($"Some tickets for EventId {command.EventId} are already refunded.");
                return Errors.Ticket.TicketAlreadyRefunded;
            }

            return (eventDetails, user, eventTicketsToRefund);
        }

        private async Task<ErrorOr<List<EventTicket>>> ProcessTicketRefund(Events eventDetails, List<EventTicket> eventTicketsToRefund)
        {
            var ticketIds = eventTicketsToRefund.Select(t => t.TicketId).ToList();
            var tickets =  _ticketRepository.GetTicketsByIds(ticketIds);
            if (tickets.Count != ticketIds.Count)
            {
                _logger.LogError($"Some tickets not found for IDs: {string.Join(", ", ticketIds.Except(tickets.Select(t => t.Id)))}.");
                return Errors.Ticket.TicketNotFound;
            }

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var paymentIds = eventTicketsToRefund.Select(t => t.PaymentId).Distinct().ToList();
                foreach (var paymentId in paymentIds)
                {
                    var payment =  _paymentRepository.GetPaymentById(paymentId);
                    if (payment == null)
                    {
                        _logger.LogError($"Payment with ID {paymentId} not found.");
                        return Errors.Payment.PaymentNotFound;
                    }

                    if (string.IsNullOrEmpty(payment.TransactionId))
                    {
                        _logger.LogError($"Payment ID {paymentId} has no associated transaction.");
                        return Errors.Payment.InvalidPaymentTransaction;
                    }

                    var refundResult = await _revenueCatServices.RefundTransactionAsync(payment.TransactionId);
                    if (refundResult.IsError)
                    {
                        _logger.LogError($"Failed to refund transaction {payment.TransactionId} for payment ID {paymentId}.");
                        return refundResult.Errors;
                    }
                }

                foreach (var eventTicket in eventTicketsToRefund)
                {
                    eventTicket.IsRefunded = true;
                     _ticketRepository.UpdateEventTicket(eventTicket);
                }

                foreach (var ticket in tickets)
                {
                    ticket.SoldOut = false;
                     _ticketRepository.Update(ticket);
                }

                eventDetails.Capacity += eventTicketsToRefund.Count;
                eventDetails.OccupiedSeats.RemoveAll(seat => eventTicketsToRefund.Any(t => t.SeatNumber == seat));
                _eventRepository.Update(eventDetails);

                transactionScope.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process refund: {ex.Message}");
                return Error.Failure(description: $"Failed to process refund: {ex.Message}");
            }

            _logger.LogInfo($"Refunded {eventTicketsToRefund.Count} tickets for EventId {eventDetails.Id}.");
            return eventTicketsToRefund;
        }

        private (TicketRefundedEvent BuyerNotification, TicketRefundedEvent OrganizerNotification) GenerateRefundNotifications(
            User user, Events eventDetails, int ticketCount)
        {
            var ticketWord = ticketCount == 1 ? "ticket" : "tickets";
            var fullName = $"{user.FirstName} {user.LastName}".Trim();

            var buyerNotificationTitle = "Your Ticket Refund Confirmation";
            var buyerNotificationMessage = $"Dear {fullName}, your {ticketCount} {ticketWord} for '{eventDetails.EventTitle}' on {eventDetails.StartDate:MMMM dd, yyyy} have been successfully refunded.";

            var buyerNotification = new TicketRefundedEvent(
                user.Id,
                eventDetails.UserId,
                eventDetails.Id,
                ticketCount,
                eventDetails.EventTitle,
                buyerNotificationTitle,
                buyerNotificationMessage,
                eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                fullName
            );

            var organizerNotificationTitle = "Ticket Refund Notification";
            var organizerNotificationMessage = $"{fullName}'s {ticketCount} {ticketWord} for your event '{eventDetails.EventTitle}' on {eventDetails.StartDate:MMMM dd, yyyy} have been successfully refunded. The event capacity has been updated. Please check your event dashboard for details.";

            var organizerNotification = new TicketRefundedEvent(
                eventDetails.UserId,
                eventDetails.UserId,
                eventDetails.Id,
                ticketCount,
                eventDetails.EventTitle,
                organizerNotificationTitle,
                organizerNotificationMessage,
                eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                fullName
            );

            return (buyerNotification, organizerNotification);
        }
    }
}