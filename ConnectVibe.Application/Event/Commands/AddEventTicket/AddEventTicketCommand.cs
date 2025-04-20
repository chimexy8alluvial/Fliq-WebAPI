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
using System.Transactions;
using System.Globalization;

namespace Fliq.Application.Event.Commands.AddEventTicket
{
    public class AddEventTicketCommand : IRequest<ErrorOr<CreateEventTicketResult>>
    {
        public int UserId { get; set; }
        public List<PurchaseTicketDetail> PurchaseTicketDetail { get; set; } = default!;
        public int PaymentId { get; set; }
    }

    public class ValidationResult
    {
        public List<Ticket> Tickets { get; init; }
        public int TotalTicketsRequested { get; init; }
        public Events EventDetails { get; init; }
        public User Buyer { get; init; }
        public Payment Payment { get; init; }

        public ValidationResult(
            List<Ticket> tickets,
            int totalTicketsRequested,
            Events eventDetails,
            User buyer,
            Payment payment)
        {
            Tickets = tickets ?? throw new ArgumentNullException(nameof(tickets));
            TotalTicketsRequested = totalTicketsRequested;
            EventDetails = eventDetails ?? throw new ArgumentNullException(nameof(eventDetails));
            Buyer = buyer ?? throw new ArgumentNullException(nameof(buyer));
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
        }
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
            // Validate request
            var validationResult = await ValidateRequest(command);
            if (validationResult.IsError)
                return validationResult.FirstError;

            var result = validationResult.Value;

            // Process ticket purchase
            var newTicketsResult = await ProcessTicketPurchase(command, result.Tickets, result.EventDetails, result.TotalTicketsRequested);
            if (newTicketsResult.IsError)
                return newTicketsResult.FirstError;

            var newTickets = newTicketsResult.Value;

            // Publish notifications
            await PublishNotifications(command, result.EventDetails, result.Buyer, result.Tickets, cancellationToken);

            _logger.LogInfo($"Assigned {newTickets.Count} tickets for event ID {result.EventDetails.Id}.");
            return new CreateEventTicketResult(newTickets);
        }

        private async Task<ErrorOr<ValidationResult>> ValidateRequest(AddEventTicketCommand command)
        {
            // Validate command
            if (command.PurchaseTicketDetail == null || !command.PurchaseTicketDetail.Any() ||
                !command.PurchaseTicketDetail.All(ptd => ptd.TicketId > 0))
            {
                _logger.LogError("No valid ticket IDs provided in the command.");
                return Errors.Ticket.NoTicketsSpecified;
            }

            // Validate tickets
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

            // Validate event
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

            // Validate buyer
            var buyer = _userRepository.GetUserById(command.UserId);
            if (buyer == null)
            {
                _logger.LogError($"Buyer with ID {command.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            // Validate payment
            var payment = _paymentRepository.GetPaymentById(command.PaymentId);
            if (payment == null)
            {
                _logger.LogError($"Payment with ID {command.PaymentId} not found.");
                return Errors.Payment.PaymentNotFound;
            }

            return new ValidationResult(tickets, totalTicketsRequested, eventDetails, buyer, payment);
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

            // Use transaction to ensure atomicity
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                ticketsToPurchase.ForEach(t => t.SoldOut = true);
                await _ticketRepository.UpdateRangeAsync(ticketsToPurchase);
                await _ticketRepository.AddEventTicketsAsync(newTickets);

                eventDetails.Capacity -= totalTicketsRequested;
                eventDetails.OccupiedSeats.AddRange(Enumerable.Range(startingSeatNumber, totalTicketsRequested));
                _eventRepository.Update(eventDetails);

                transactionScope.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process ticket purchase: {ex.Message}");
                return Error.Failure(description: $"Failed to process ticket purchase: {ex.Message}");
            }

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
                    TicketCategory = ticket.TicketCategory.ToString()
                })
                .ToList();

            var notificationTasks = new List<Task>();

            // User notifications (excluding buyer)
            foreach (var ta in ticketAssignments.Where(ta => ta.UserId.HasValue && ta.UserId > 0 && ta.UserId != command.UserId))
            {
                notificationTasks.Add(PublishWithErrorHandling(
                    new TicketPurchasedEvent(
                        userId: ta.UserId!.Value,
                        buyerId: command.UserId,
                        organizerId: eventDetails.UserId,
                        eventId: eventDetails.Id,
                        eventTitle: eventDetails.EventTitle,
                        eventDate: eventDetails.StartDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture),
                        buyerName: buyer.DisplayName,
                        ticketCategory: ta.TicketCategory!, // Updated from ticketType
                        title: "Your Ticket Purchase Confirmation",
                        message: $"A {ta.TicketCategory} ticket has been purchased for you for '{eventDetails.EventTitle}' on {eventDetails.StartDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)} by {buyer.DisplayName}."
                    ),
                    cancellationToken
                ));
            }

            // Email notifications
            foreach (var ta in ticketAssignments.Where(ta => !ta.UserId.HasValue && !string.IsNullOrEmpty(ta.Email)))
            {
                notificationTasks.Add(PublishWithErrorHandling(
                    new TicketPurchasedEvent(
                        email: ta.Email!,
                        buyerId: command.UserId,
                        organizerId: eventDetails.UserId,
                        eventId: eventDetails.Id,
                        eventTitle: eventDetails.EventTitle,
                        eventDate: eventDetails.StartDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture),
                        buyerName: buyer.DisplayName,
                        ticketCategory: ta.TicketCategory!, // Updated from ticketType
                        title: "Your Ticket Purchase Confirmation",
                        message: $"A {ta.TicketCategory} ticket has been purchased for you for '{eventDetails.EventTitle}' on {eventDetails.StartDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)} by {buyer.DisplayName}."
                    ),
                    cancellationToken
                ));
            }

            // Buyer-specific notification for their own tickets
            var buyerTickets = ticketAssignments.Where(ta => ta.UserId == command.UserId).ToList();
            if (buyerTickets.Any())
            {
                var buyerTicketTypes = string.Join(", ", buyerTickets.Select(bt => $"{bt.TicketCategory}"));
                notificationTasks.Add(PublishWithErrorHandling(
                    new TicketPurchasedEvent(
                        userId: command.UserId,
                        buyerId: command.UserId,
                        organizerId: eventDetails.UserId,
                        eventId: eventDetails.Id,
                        eventTitle: eventDetails.EventTitle,
                        eventDate: eventDetails.StartDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture),
                        buyerName: buyer.DisplayName,
                        ticketCategory: buyerTickets.First().TicketCategory!, // Updated from ticketType
                        title: "Your Personal Ticket Confirmation",
                        message: $"You have been assigned {buyerTickets.Count} ticket(s) for '{eventDetails.EventTitle}' on {eventDetails.StartDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)}: {buyerTicketTypes}."
                    ),
                    cancellationToken
                ));
            }

            // General buyer notification (summary)
            var ticketCategoriesCount = ticketsToPurchase
                .GroupBy(t => t.TicketCategory)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());
            var ticketBreakdown = string.Join(", ", ticketCategoriesCount.Select(kv => $"{kv.Value} {kv.Key} ticket{(kv.Value > 1 ? "s" : "")}"));
            var assignedTo = string.Join(", ", ticketAssignments.Select(ta => ta.UserId.HasValue ? $"User {ta.UserId}: {ta.TicketCategory}" : $"{ta.Email}: {ta.TicketCategory}"));

            notificationTasks.Add(PublishWithErrorHandling(
                new TicketPurchasedEvent(
                    buyerId: command.UserId,
                    organizerId: eventDetails.UserId,
                    eventId: eventDetails.Id,
                    numberOfTickets: ticketAssignments.Count,
                    eventTitle: eventDetails.EventTitle,
                    eventDate: eventDetails.StartDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture),
                    buyerName: buyer.DisplayName,
                    ticketDetails: ticketAssignments,
                    title: "New Tickets Purchased",
                    message: $"You have successfully purchased {ticketAssignments.Count} ticket(s) for the event '{eventDetails.EventTitle}' on {eventDetails.StartDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)}.\nBreakdown: {ticketBreakdown}\nAssigned to: {assignedTo}"
                ),
                cancellationToken
            ));

            await Task.WhenAll(notificationTasks);
        }

        private async Task PublishWithErrorHandling(TicketPurchasedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Publish(notification, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to publish notification for event {notification.EventId}: {ex.Message}");
            }
        }
    }
}