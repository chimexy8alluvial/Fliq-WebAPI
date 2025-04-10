using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Contracts.Event;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;

namespace Fliq.Application.Event.Commands.AddEventTicket
{
    public class AddEventTicketCommand : IRequest<ErrorOr<CreateEventTicketResult>>
    {
        public int UserId { get; set; }
        public List<PurchaseTicketDetail> PurchaseTicketDetail { get; set; } = default!;
        public int PaymentId { get; set; }
    }

    public class AddEventTicketCommandHandler : IRequestHandler<AddEventTicketCommand, ErrorOr<CreateEventTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly ITicketRepository _ticketRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public AddEventTicketCommandHandler(
            IEventRepository eventRepository,
            ILoggerManager logger,
            ITicketRepository ticketRepository,
            IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            IMediator mediator)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<CreateEventTicketResult>> Handle(AddEventTicketCommand command, CancellationToken cancellationToken)
        {
            if (command.PurchaseTicketDetail == null || !command.PurchaseTicketDetail.Any() ||
                !command.PurchaseTicketDetail.All(ptd => ptd.TicketId > 0))
            {
                _logger.LogError("No valid ticket IDs provided in the command.");
                return Errors.Ticket.NoTicketsSpecified;
            }

            var ticketIds = command.PurchaseTicketDetail.Select(ptd => ptd.TicketId).ToList();
            var ticketsToPurchase = _ticketRepository.GetTicketsByIds(ticketIds);

            if (ticketsToPurchase.Count != ticketIds.Count)
            {
                var missingIds = ticketIds.Except(ticketsToPurchase.Select(t => t.Id)).ToList();
                _logger.LogError($"Tickets not found for IDs: {string.Join(", ", missingIds)}.");
                return Errors.Ticket.TicketNotFound;
            }

            var eventId = ticketsToPurchase.First().EventId;
            if (ticketsToPurchase.Any(t => t.EventId != eventId))
            {
                _logger.LogError("Tickets belong to multiple events, which is not supported.");
                return Errors.Ticket.MultipleEventsNotSupported;
            }

            var eventDetails = _eventRepository.GetEventById(eventId);
            if (eventDetails == null || eventDetails.IsDeleted)
            {
                _logger.LogError($"Event with ID {eventId} not found.");
                return Errors.Event.EventNotFound;
            }

            if (eventDetails.Status == EventStatus.Cancelled)
            {
                _logger.LogError($"Event with ID {eventId} has been cancelled.");
                return Errors.Event.EventCancelledAlready;
            }

            if (eventDetails.Status == EventStatus.Past)
            {
                _logger.LogError($"Event with ID {eventId} has ended.");
                return Errors.Event.EventEndedAlready;
            }

            var buyer = _userRepository.GetUserById(command.UserId);
            if (buyer == null)
            {
                _logger.LogError($"Buyer with ID {command.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            int totalTicketsRequested = ticketIds.Count;
            if (eventDetails.Capacity < totalTicketsRequested)
            {
                _logger.LogError("Insufficient capacity for requested number of tickets.");
                return Errors.Event.InsufficientCapacity;
            }

            if (ticketsToPurchase.Any(t => t.SoldOut))
            {
                var soldOutIds = ticketsToPurchase.Where(t => t.SoldOut).Select(t => t.Id).ToList();
                _logger.LogInfo($"Tickets already sold out: {string.Join(", ", soldOutIds)}.");
                return Errors.Ticket.TicketAlreadySoldOut;
            }

            var payment = _paymentRepository.GetPaymentById(command.PaymentId);
            if (payment == null)
            {
                _logger.LogError($"Payment with ID {command.PaymentId} not found.");
                return Errors.Payment.PaymentNotFound;
            }

            int startingSeatNumber = eventDetails.OccupiedSeats.Any() ? eventDetails.OccupiedSeats.Max() + 1 : 1;
            if (startingSeatNumber + totalTicketsRequested - 1 > eventDetails.Capacity)
            {
                _logger.LogError("Not enough available seats left.");
                return Errors.Event.NoAvailableSeats;
            }

            var newTickets = ticketsToPurchase.Select((ticket, index) => new EventTicket
            {
                TicketId = ticket.Id,
                UserId = command.PurchaseTicketDetail[index].UserId ?? 0,
                PaymentId = command.PaymentId,
                SeatNumber = startingSeatNumber + index
            }).ToList();

            foreach (var ticket in ticketsToPurchase)
            {
                ticket.SoldOut = true;
            }

            _ticketRepository.UpdateRange(ticketsToPurchase);
            _ticketRepository.AddEventTickets(newTickets);
            eventDetails.Capacity -= totalTicketsRequested;
            eventDetails.OccupiedSeats.AddRange(Enumerable.Range(startingSeatNumber, totalTicketsRequested));
            _eventRepository.Update(eventDetails);

            _logger.LogInfo($"Assigned {newTickets.Count} tickets for event ID {eventId}.");

            var ticketAssignments = command.PurchaseTicketDetail
                .Zip(ticketsToPurchase, (detail, ticket) => new TicketDetail
                {
                    UserId = detail.UserId,
                    Email = detail.Email,
                    TicketType = ticket.TicketType.ToString()
                })
                .ToList();

            foreach (var assignment in ticketAssignments.Where(ta => ta.UserId.HasValue && ta.UserId > 0 /*&& ta.UserId != command.UserId*/))
            {
                var userNotificationTitle = "Your Ticket Purchase Confirmation";
                var userNotificationMessage = $"A {assignment.TicketType} ticket has been purchased for you" +
                                             $"for '{eventDetails.EventTitle}' on {eventDetails.StartDate} " +
                                             $"by {buyer.DisplayName}.";

                await _mediator.Publish(new TicketPurchasedEvent(
                    assignment.UserId!.Value,
                    command.UserId,
                    eventDetails.UserId,
                    eventId,
                    eventDetails.EventTitle,
                    eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                    buyer.DisplayName,
                    assignment.TicketType!,
                    userNotificationTitle,
                    userNotificationMessage
                ), cancellationToken);
            }

            foreach (var assignment in ticketAssignments.Where(ta => !ta.UserId.HasValue && !string.IsNullOrEmpty(ta.Email)))
            {
                var emailNotificationTitle = "Your Ticket Purchase Confirmation";
                var emailNotificationMessage = $"A {assignment.TicketType} ticket has been purchased for you " +
                                              $"for '{eventDetails.EventTitle}' on {eventDetails.StartDate} " +
                                              $"by {buyer.DisplayName}.";

                await _mediator.Publish(new TicketPurchasedEvent(
                    assignment.Email!,
                    command.UserId,
                    eventDetails.UserId,
                    eventId,
                    eventDetails.EventTitle,
                    eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                    buyer.DisplayName,
                    assignment.TicketType!,
                    emailNotificationTitle,
                    emailNotificationMessage
                ), cancellationToken);
            }

            var ticketTypesCount = ticketsToPurchase
                 .GroupBy(t => t.TicketType)
                 .ToDictionary(g => g.Key.ToString(), g => g.Count());
            var ticketBreakdown = string.Join(", ", ticketTypesCount.Select(kv => $"{kv.Value} {kv.Key} ticket{(kv.Value > 1 ? "s" : "")}"));
            var buyerNotificationTitle = "New Tickets Purchased";
            var buyerNotificationMessage = $"You have successfully purchased {newTickets.Count} ticket(s) " +
                                          $"for the event '{eventDetails.EventTitle}' on {eventDetails.StartDate}.\n" +
                                          $"Breakdown: {ticketBreakdown}\n" +
                                          $"Assigned to: {string.Join(", ", ticketAssignments.Select(ta => ta.UserId.HasValue ? $"User {ta.UserId}: {ta.TicketType}" : $"{ta.Email}: {ta.TicketType}"))}";

            await _mediator.Publish(new TicketPurchasedEvent(
                command.UserId,
                eventDetails.UserId,
                eventId,
                totalTicketsRequested,
                eventDetails.EventTitle,
                eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                buyer.DisplayName,
                ticketAssignments,
                buyerNotificationTitle,
                buyerNotificationMessage
            ), cancellationToken);

            return new CreateEventTicketResult(newTickets);
        }
    }
}