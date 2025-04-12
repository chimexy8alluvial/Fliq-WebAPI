using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Contracts.Event;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
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
            // Validate command
            var validationResult = ValidateCommand(command);
            if (validationResult.IsError)
                return validationResult.FirstError;

            // Extract and validate tickets
            var ticketsResult = await ExtractAndValidateTickets(command);
            if (ticketsResult.IsError)
                return ticketsResult.FirstError;

            var (ticketsToPurchase, totalTicketsRequested) = ticketsResult.Value;

            // Validate event
            var eventResult = await ValidateEvent(ticketsToPurchase.First().EventId);
            if (eventResult.IsError)
                return eventResult.FirstError;

            var eventDetails = eventResult.Value;

            // Validate buyer
            var buyerResult = ValidateBuyer(command.UserId);
            if (buyerResult.IsError)
                return buyerResult.FirstError;

            var buyer = buyerResult.Value;

            // Validate payment
            var paymentResult = ValidatePayment(command.PaymentId);
            if (paymentResult.IsError)
                return paymentResult.FirstError;

            // Process ticket purchase
            var newTicketsResult = await ProcessTicketPurchase(command, ticketsToPurchase, eventDetails, totalTicketsRequested);
            if (newTicketsResult.IsError)
                return newTicketsResult.FirstError;

            var newTickets = newTicketsResult.Value;

            // Publish notifications
            await PublishNotifications(command, eventDetails, buyer, ticketsToPurchase, cancellationToken);

            _logger.LogInfo($"Assigned {newTickets.Count} tickets for event ID {eventDetails.Id}.");
            return new CreateEventTicketResult(newTickets);
        }

        private ErrorOr<Success> ValidateCommand(AddEventTicketCommand command)
        {
            if (command.PurchaseTicketDetail == null || !command.PurchaseTicketDetail.Any() ||
                !command.PurchaseTicketDetail.All(ptd => ptd.TicketId > 0))
            {
                _logger.LogError("No valid ticket IDs provided in the command.");
                return Errors.Ticket.NoTicketsSpecified;
            }
            return Result.Success;
        }

        private async Task<ErrorOr<(List<Ticket> Tickets, int TotalTicketsRequested)>> ExtractAndValidateTickets(AddEventTicketCommand command)
        {
            var ticketIds = command.PurchaseTicketDetail.Select(ptd => ptd.TicketId).ToList();
            var totalTicketsRequested = ticketIds.Count;

            var tickets = await _ticketRepository.GetTicketsByIdsAsync(ticketIds);
            if (tickets.Count != ticketIds.Count)
            {
                var missingIds = ticketIds.Except(tickets.Select(t => t.Id));
                _logger.LogError($"Tickets not found for IDs: {string.Join(", ", missingIds)}.");
                return Errors.Ticket.TicketNotFound;
            }

            var eventId = tickets.First().EventId;
            if (tickets.Any(t => t.EventId != eventId))
            {
                _logger.LogError("Tickets belong to multiple events, which is not supported.");
                return Errors.Ticket.MultipleEventsNotSupported;
            }

            if (tickets.Any(t => t.SoldOut))
            {
                var soldOutIds = tickets.Where(t => t.SoldOut).Select(t => t.Id);
                _logger.LogInfo($"Tickets already sold out: {string.Join(", ", soldOutIds)}.");
                return Errors.Ticket.TicketAlreadySoldOut;
            }

            return (tickets, totalTicketsRequested);
        }

        private async Task<ErrorOr<Events>> ValidateEvent(int eventId)
        {
            var eventDetails = await _eventRepository.GetEventByIdAsync(eventId);
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

            return eventDetails;
        }

        private ErrorOr<User> ValidateBuyer(int userId)
        {
            var buyer = _userRepository.GetUserById(userId);
            if (buyer == null)
            {
                _logger.LogError($"Buyer with ID {userId} not found.");
                return Errors.User.UserNotFound;
            }
            return buyer;
        }

        private ErrorOr<Payment> ValidatePayment(int paymentId)
        {
            var payment = _paymentRepository.GetPaymentById(paymentId);
            if (payment == null)
            {
                _logger.LogError($"Payment with ID {paymentId} not found.");
                return Errors.Payment.PaymentNotFound;
            }
            return payment;
        }

        private async Task<ErrorOr<List<EventTicket>>> ProcessTicketPurchase(
            AddEventTicketCommand command,
            List<Ticket> ticketsToPurchase,
            Events eventDetails,
            int totalTicketsRequested)
        {
            if (eventDetails.Capacity < totalTicketsRequested)
            {
                _logger.LogError("Insufficient capacity for requested number of tickets.");
                return Errors.Event.InsufficientCapacity;
            }

            int startingSeatNumber = eventDetails.OccupiedSeats.Any() ? eventDetails.OccupiedSeats.Max() + 1 : 1;
            if (startingSeatNumber + totalTicketsRequested - 1 > eventDetails.Capacity)
            {
                _logger.LogError("Not enough available seats left.");
                return Errors.Event.NoAvailableSeats;
            }

            var newTickets = command.PurchaseTicketDetail
                .Select((detail, index) => new EventTicket
                {
                    TicketId = detail.TicketId,
                    UserId = detail.UserId ?? 0,
                    PaymentId = command.PaymentId,
                    SeatNumber = startingSeatNumber + index
                })
                .ToList();

            ticketsToPurchase.ForEach(t => t.SoldOut = true);
            await _ticketRepository.UpdateRangeAsync(ticketsToPurchase);
            await _ticketRepository.AddEventTicketsAsync(newTickets);

            eventDetails.Capacity -= totalTicketsRequested;
            eventDetails.OccupiedSeats.AddRange(Enumerable.Range(startingSeatNumber, totalTicketsRequested));
             _eventRepository.Update(eventDetails);

            return newTickets;
        }

        private async Task PublishNotifications(
            AddEventTicketCommand command,
            Events eventDetails,
            User buyer,
            List<Ticket> ticketsToPurchase,
            CancellationToken cancellationToken)
        {
            var ticketAssignments = command.PurchaseTicketDetail
                .Zip(ticketsToPurchase, (detail, ticket) => new TicketDetail
                {
                    UserId = detail.UserId,
                    Email = detail.Email,
                    TicketType = ticket.TicketType.ToString()
                })
                .ToList();

            var notificationTasks = new List<Task>();

            // User notifications (excluding buyer)
            notificationTasks.AddRange(ticketAssignments
                .Where(ta => ta.UserId.HasValue && ta.UserId > 0 && ta.UserId != command.UserId)
                .Select(ta => _mediator.Publish(new TicketPurchasedEvent(
                    ta.UserId!.Value,
                    command.UserId,
                    eventDetails.UserId,
                    eventDetails.Id,
                    eventDetails.EventTitle,
                    eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                    buyer.DisplayName,
                    ta.TicketType!,
                    "Your Ticket Purchase Confirmation",
                    $"A {ta.TicketType} ticket has been purchased for you for '{eventDetails.EventTitle}' on {eventDetails.StartDate} by {buyer.DisplayName}."
                ), cancellationToken)));

            // Email notifications
            notificationTasks.AddRange(ticketAssignments
                .Where(ta => !ta.UserId.HasValue && !string.IsNullOrEmpty(ta.Email))
                .Select(ta => _mediator.Publish(new TicketPurchasedEvent(
                    ta.Email!,
                    command.UserId,
                    eventDetails.UserId,
                    eventDetails.Id,
                    eventDetails.EventTitle,
                    eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                    buyer.DisplayName,
                    ta.TicketType!,
                    "Your Ticket Purchase Confirmation",
                    $"A {ta.TicketType} ticket has been purchased for you for '{eventDetails.EventTitle}' on {eventDetails.StartDate} by {buyer.DisplayName}."
                ), cancellationToken)));

            // Buyer-specific notification for their own tickets
            var buyerTickets = ticketAssignments.Where(ta => ta.UserId == command.UserId).ToList();
            if (buyerTickets.Any())
            {
                var buyerTicketTypes = string.Join(", ", buyerTickets.Select(bt => $"{bt.TicketType}"));
                notificationTasks.Add(_mediator.Publish(new TicketPurchasedEvent(
                    command.UserId,
                    command.UserId,
                    eventDetails.UserId,
                    eventDetails.Id,
                    eventDetails.EventTitle,
                    eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                    buyer.DisplayName,
                    buyerTickets.First().TicketType!,
                    "Your Personal Ticket Confirmation",
                    $"You have been assigned {buyerTickets.Count} ticket(s) for '{eventDetails.EventTitle}' on {eventDetails.StartDate}: {buyerTicketTypes}."
                ), cancellationToken));
            }

            // General buyer notification (summary)
            var ticketTypesCount = ticketsToPurchase
                .GroupBy(t => t.TicketType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());
            var ticketBreakdown = string.Join(", ", ticketTypesCount.Select(kv => $"{kv.Value} {kv.Key} ticket{(kv.Value > 1 ? "s" : "")}"));
            var assignedTo = string.Join(", ", ticketAssignments.Select(ta => ta.UserId.HasValue ? $"User {ta.UserId}: {ta.TicketType}" : $"{ta.Email}: {ta.TicketType}"));

            notificationTasks.Add(_mediator.Publish(new TicketPurchasedEvent(
                command.UserId,
                eventDetails.UserId,
                eventDetails.Id,
                ticketAssignments.Count,
                eventDetails.EventTitle,
                eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                buyer.DisplayName,
                ticketAssignments,
                "New Tickets Purchased",
                $"You have successfully purchased {ticketAssignments.Count} ticket(s) for the event '{eventDetails.EventTitle}' on {eventDetails.StartDate}.\nBreakdown: {ticketBreakdown}\nAssigned to: {assignedTo}"
            ), cancellationToken));

            await Task.WhenAll(notificationTasks);
        }
    }
}