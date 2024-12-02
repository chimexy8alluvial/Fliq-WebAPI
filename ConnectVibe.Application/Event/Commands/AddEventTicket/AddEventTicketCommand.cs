using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Event.Commands.AddEventTicket
{
    public class AddEventTicketCommand : IRequest<ErrorOr<CreateEventTicketResult>>
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public int PaymentId { get; set; }
        public int NumberOfTickets { get; set; }  // New property for multiple tickets
    }

    public class AddEventTicketCommandHandler : IRequestHandler<AddEventTicketCommand, ErrorOr<CreateEventTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ITicketRepository _ticketRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public AddEventTicketCommandHandler(
            IEventRepository eventRepository,
            ILoggerManager logger,
            IMapper mapper,
            ITicketRepository ticketRepository,
            IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            IMediator mediator)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _mapper = mapper;
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<CreateEventTicketResult>> Handle(AddEventTicketCommand command, CancellationToken cancellationToken)
        {
            var eventDetails = _eventRepository.GetEventById(command.TicketId); //this method expects eventId not ticketId
            if (eventDetails == null)
            {
                _logger.LogError($"Event with ID {command.TicketId} not found.");
                return Errors.Event.EventNotFound;
            }

            //Fetch Buyer Details
            var buyer = _userRepository.GetUserById(command.UserId);
            if(buyer == null)
            {
                _logger.LogError($"Buyer with ID {command.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            if (eventDetails.Capacity < command.NumberOfTickets)
            {
                _logger.LogError("Insufficient capacity for requested number of tickets.");
                return Errors.Event.InsufficientCapacity;
            }

            var payment = _paymentRepository.GetPaymentById(command.PaymentId);
            if (payment == null)
            {
                _logger.LogError($"Payment with ID {command.PaymentId} not found.");
                return Errors.Payment.PaymentNotFound;
            }

            var newTickets = new List<EventTicket>();

            int startingSeatNumber = eventDetails.OccupiedSeats.Any() ? eventDetails.OccupiedSeats.Max() + 1 : 1;

            for (int i = 0; i < command.NumberOfTickets; i++)
            {
                if (startingSeatNumber > eventDetails.Capacity)
                {
                    _logger.LogError("No available seats left.");
                    return Errors.Event.NoAvailableSeats;
                }

                var newEventTicket = new EventTicket
                {
                    TicketId = command.TicketId,
                    UserId = command.UserId,
                    PaymentId = command.PaymentId,
                    SeatNumber = startingSeatNumber,
                };

                eventDetails.OccupiedSeats.Add(startingSeatNumber);
                newTickets.Add(newEventTicket);
                _ticketRepository.AddEventTicket(newEventTicket);

                startingSeatNumber++;
            }

            eventDetails.Capacity -= command.NumberOfTickets;
            _eventRepository.Update(eventDetails);
            _logger.LogInfo($"Added {newTickets.Count} tickets for event ID {command.TicketId}.");

            // Prepare notification details
            var notificationTitle = "New Tickets Purchased";
            var notificationMessage = $"You have successfully purchased {newTickets.Count} ticket(s) for the event '{eventDetails.EventTitle}' on {eventDetails.StartDate}.";

            await _mediator.Publish(new TicketPurchasedEvent(
                        command.UserId,
                        eventDetails.UserId,  // Organizer ID
                        eventDetails.Id,
                        command.NumberOfTickets,
                        eventDetails.EventTitle,
                        notificationTitle,
                        notificationMessage,
                        eventDetails.StartDate.ToString("MMMM dd, yyyy"), // Format the event date
                        buyer.DisplayName
                         ), cancellationToken);

            return new CreateEventTicketResult(newTickets);
        }
    }
}