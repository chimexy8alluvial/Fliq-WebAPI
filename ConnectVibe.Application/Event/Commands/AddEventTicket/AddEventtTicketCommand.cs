using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
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
    }

    public class AddEventTicketCommandHandler : IRequestHandler<AddEventTicketCommand, ErrorOr<CreateEventTicketResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ITicketRepository _ticketRepository;
        private readonly IPaymentRepository _paymentRepository;

        public AddEventTicketCommandHandler(IEventRepository eventRepository, ILoggerManager logger, IMapper mapper, ITicketRepository ticketRepository, IPaymentRepository paymentRepository)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _mapper = mapper;
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<ErrorOr<CreateEventTicketResult>> Handle(AddEventTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = _eventRepository.GetEventById(command.TicketId);
            if (ticket == null)
            {
                _logger.LogError($"ticket with ID {command.TicketId} not found.");
                return Errors.Event.EventNotFound;
            }

            var payment = _paymentRepository.GetPaymentById(command.PaymentId);
            if (payment == null)
            {
                _logger.LogError($"payment with ID {command.PaymentId} not found.");
                return Errors.Payment.PaymentNotFound;
            }

            var newEventTicket = _mapper.Map<EventTicket>(command);

            // Add the ticket to the event's Tickets list and update

            _ticketRepository.AddEventTicket(newEventTicket);
            _logger.LogInfo($"Added EventTicket with ID {newEventTicket.Id}");
            return new CreateEventTicketResult(newEventTicket);
        }
    }
}