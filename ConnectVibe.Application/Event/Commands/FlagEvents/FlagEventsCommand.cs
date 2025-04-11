using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
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

        public FlagEventCommandHandler(
            ILoggerManager logger,
            IUserRepository userRepository,
            IEventRepository eventRepository)
        {

            _logger = logger;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
           

        }

        public async Task<ErrorOr<Unit>> Handle(FlagEventCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Flagging Event with ID: {command.EventId}");
            var eventFromDb = _eventRepository.GetEventById(command.EventId);
            if (eventFromDb == null)
            {
                _logger.LogError($"Event with ID: {command.EventId} was not found.");
                return Errors.Event.EventNotFound;
            }

            if (eventFromDb.IsFlagged)
            {
                _logger.LogInfo($"Event with ID: {command.EventId} has been flagged already.");
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


            return Unit.Value;
        }
    }
}
