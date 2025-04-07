using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.Event.Commands.AddEventTicket
{
    public class AddEventTicketCommand : IRequest<ErrorOr<CreateEventTicketResult>>
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int PaymentId { get; set; }
        public Dictionary<TicketType, int> TicketQuantities { get; set; } = default!;
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
            var eventDetails = _eventRepository.GetEventById(command.EventId);
            if (eventDetails == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            var buyer = _userRepository.GetUserById(command.UserId);
            if (buyer == null)
            {
                _logger.LogError($"Buyer with ID {command.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            int totalTicketsRequested = command.TicketQuantities.Sum(kv => kv.Value);
            if (eventDetails.Capacity < totalTicketsRequested)
            {
                _logger.LogError("Insufficient capacity for requested number of tickets.");
                return Errors.Event.InsufficientCapacity;
            }

            var tickets = _ticketRepository.GetTicketsByEventId(command.EventId);
            if (tickets == null || !tickets.Any())
            {
                _logger.LogInfo($"No tickets found for EventId {command.EventId}.");
                return Errors.Ticket.NoTicketsAvailable; // Define this error if not already present
            }

            if (tickets.All(t => t.SoldOut))
            {
                _logger.LogInfo($"All tickets for EventId {command.EventId} are already sold out.");
                return Errors.Ticket.TicketAlreadySoldOut;
            }

            var payment = _paymentRepository.GetPaymentById(command.PaymentId);
            if (payment == null)
            {
                _logger.LogError($"Payment with ID {command.PaymentId} not found.");
                return Errors.Payment.PaymentNotFound;
            }

            var newTickets = new List<EventTicket>();
            int startingSeatNumber = eventDetails.OccupiedSeats.Any() ? eventDetails.OccupiedSeats.Max() + 1 : 1;

            foreach (var ticketRequest in command.TicketQuantities)
            {
                TicketType ticketType = ticketRequest.Key;
                int quantity = ticketRequest.Value;

                var availableTicketsOfType = tickets
                    .Where(t => t.TicketType == ticketType && !t.SoldOut)
                    .ToList();

                if (availableTicketsOfType.Count < quantity)
                {
                    _logger.LogInfo($"Not enough {ticketType} tickets available for EventId {command.EventId}. Requested: {quantity}, Available: {availableTicketsOfType.Count}");
                    return Errors.Ticket.InsufficientAvailableTickets;
                }

                var ticketsToUse = availableTicketsOfType.Take(quantity).ToList();

                foreach (var ticket in ticketsToUse)
                {
                    if (startingSeatNumber > eventDetails.Capacity)
                    {
                        _logger.LogError("No available seats left.");
                        return Errors.Event.NoAvailableSeats;
                    }

                    var newEventTicket = new EventTicket
                    {
                        TicketId = ticket.Id, // Use the existing ticket’s ID
                        UserId = command.UserId,
                        PaymentId = command.PaymentId,
                        SeatNumber = startingSeatNumber
                    };

                    ticket.SoldOut = true; // Mark the ticket as sold
                    eventDetails.OccupiedSeats.Add(startingSeatNumber);
                    newTickets.Add(newEventTicket);
                    _ticketRepository.Update(ticket); // Update the existing ticket

                    startingSeatNumber++;
                }
            }

            eventDetails.Capacity -= totalTicketsRequested;
            _eventRepository.Update(eventDetails);
            _logger.LogInfo($"Assigned {newTickets.Count} existing tickets for event ID {command.EventId}.");

            var notificationTitle = "New Tickets Purchased";
            var ticketBreakdown = string.Join(", ", command.TicketQuantities.Select(kv => $"{kv.Value} {kv.Key} ticket{(kv.Value > 1 ? "s" : "")}"));
            var notificationMessage = $"You have successfully purchased {newTickets.Count} ticket(s) for the event '{eventDetails.EventTitle}' on {eventDetails.StartDate}.\nBreakdown: {ticketBreakdown}.";
           
            await _mediator.Publish(new TicketPurchasedEvent(
                        command.UserId,
                        eventDetails.UserId,  // Organizer ID
                        eventDetails.Id,
                        totalTicketsRequested,
                        eventDetails.EventTitle,
                        notificationTitle,
                        notificationMessage,
                        eventDetails.StartDate.ToString("MMMM dd, yyyy"),
                        buyer.DisplayName
                         ), cancellationToken);

            return new CreateEventTicketResult(newTickets);
        }
    }
}