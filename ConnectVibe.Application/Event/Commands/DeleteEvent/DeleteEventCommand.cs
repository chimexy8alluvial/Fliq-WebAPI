using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.EventServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Event.Commands.DeleteEvent
{
    public record DeleteEventCommand(int EventId) : IRequest<ErrorOr<Unit>>;
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, ErrorOr<Unit>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMediaServices _mediaServices;
        private readonly IEventRepository _eventRepository;
        private readonly ILocationService _locationService;
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;
        private readonly IEventService _eventService;
        private const string _eventDocument = "Event Documents";

        public DeleteEventCommandHandler(
            IMapper mapper,
            ILoggerManager logger,
            IUserRepository userRepository,
            IMediaServices mediaServices,
            IEventRepository eventRepository,
            ILocationService locationService,
            IMediator mediator,
            IEmailService emailService,
            IEventService eventService)
        {
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _mediaServices = mediaServices;
            _eventRepository = eventRepository;
            _locationService = locationService;
            _mediator = mediator;
            _emailService = emailService;
            _eventService = eventService;
        }

        public async Task<ErrorOr<Unit>> Handle(DeleteEventCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Deleting event with ID: {command.EventId}");

            var eventFromDb = _eventRepository.GetEventById(command.EventId);
            if (eventFromDb == null)
            {
                _logger.LogError($"Event with ID: {command.EventId} was not found.");
                return Errors.Event.EventNotFound;
            }

            if (eventFromDb.IsDeleted)
            {
                _logger.LogError($"Event with ID: {command.EventId} has been deleted already.");
                return Errors.Event.EventDeletedAlready;
            }

            var user = _userRepository.GetUserById(eventFromDb.UserId);
            if (user == null)
            {
                _logger.LogError($"User with Id: {eventFromDb.UserId} was not found.");
                return Errors.User.UserNotFound;
            }

            eventFromDb.IsFlagged = true;

            _eventRepository.Update(eventFromDb);

            _logger.LogInfo($"Event with ID: {command.EventId} was deleted");


            var organizerName = $"{user.FirstName} {user.LastName}";

            await _mediator.Publish(new EventCreatedEvent(
                user.Id,
                eventFromDb.Id,
                user.Id,
                organizerName,
                Enumerable.Empty<int>(), // Organizer-only notification
                "Event Deleted",
                $"Your event '{eventFromDb.EventTitle}' has been deleted!",
                false,
                null,
                null

            ), cancellationToken);

            return Unit.Value;
        }
    }
}
