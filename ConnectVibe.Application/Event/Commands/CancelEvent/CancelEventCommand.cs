using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Event.Commands.CancelEvent
{
    public record CancelEventCommand(int EventId) : IRequest<ErrorOr<Unit>>;
    public class CancelEventCommandHandler : IRequestHandler<CancelEventCommand, ErrorOr<Unit>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMediator _mediator;
        private const string _eventDocument = "Event Documents";

        public CancelEventCommandHandler(
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

        public async Task<ErrorOr<Unit>> Handle(CancelEventCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Cancelling Event with ID: {command.EventId}");
            var eventFromDb = _eventRepository.GetEventById(command.EventId);
            if (eventFromDb == null)
            {
                _logger.LogError($"Event with ID: {command.EventId} was not found.");
                return Errors.Event.EventNotFound;
            }

            if (eventFromDb.IsCancelled)
            {
                _logger.LogError($"Event with ID: {command.EventId} has been cancelled already.");
                return Errors.Event.EventCancelledAlready;
            }

            var user = _userRepository.GetUserById(eventFromDb.UserId);
            if (user == null)
            {
                _logger.LogError($"User with Id: {eventFromDb.UserId} was not found.");
                return Errors.User.UserNotFound;
            }

            eventFromDb.IsFlagged = true;

            _eventRepository.Update(eventFromDb);

            _logger.LogInfo($"Event with ID: {command.EventId} was cancelled");


            var organizerName = $"{user.FirstName} {user.LastName}";

            await _mediator.Publish(new EventCreatedEvent(
                user.Id,
                eventFromDb.Id,
                user.Id,
                organizerName,
                Enumerable.Empty<int>(), // Organizer-only notification
                "Event Cancelled",
                $"Your event '{eventFromDb.EventTitle}' has been Cancelled!",
                false,
                null,
                null

            ), cancellationToken);            

            return Unit.Value;
        }


       
    }
}
