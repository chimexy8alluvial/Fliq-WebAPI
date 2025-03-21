using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Event.Commands.FlagEvent
{
    public record FlagEventCommand(int EventId) : IRequest<ErrorOr<Unit>>;
    public class FlagEventCommandHandler : IRequestHandler<FlagEventCommand, ErrorOr<Unit>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMediator _mediator;
        private const string _eventDocument = "Event Documents";

        public FlagEventCommandHandler(
            ILoggerManager logger,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IMediator mediator)
        {

            _logger = logger;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _mediator = mediator;

        }

        public async Task<ErrorOr<Unit>> Handle(FlagEventCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Flagging Event with ID: {command.EventId}");
            var eventFromDb = _eventRepository.GetEventById(command.EventId);
            if (eventFromDb == null)
            {
                _logger.LogError($"Event with ID: {command.EventId} was not found.");
                return Errors.Event.EventNotFound;
            }

            if (eventFromDb.IsFlagged)
            {
                _logger.LogError($"Event with ID: {command.EventId} has been flagged already.");
                return Errors.Event.EventFlaggedAlready;
            }

            var user = _userRepository.GetUserById(eventFromDb.UserId);
            if (user == null)
            {
                _logger.LogError($"User with Id: {eventFromDb.UserId} was not found.");
                return Errors.User.UserNotFound;
            }

            eventFromDb.IsFlagged = true;

            _eventRepository.Update(eventFromDb);

            _logger.LogInfo($"Event with ID: {command.EventId} was flagged");


            var organizerName = $"{user.FirstName} {user.LastName}";

            await _mediator.Publish(new EventCreatedEvent(
                user.Id,
                eventFromDb.Id,
                user.Id,
                organizerName,
                Enumerable.Empty<int>(), // Organizer-only notification
                "Event Flagged",
                $"Your event '{eventFromDb.EventTitle}' has been flagged!",
                false,
                null,
                null

            ), cancellationToken);

            return Unit.Value;
        }
    }
}
