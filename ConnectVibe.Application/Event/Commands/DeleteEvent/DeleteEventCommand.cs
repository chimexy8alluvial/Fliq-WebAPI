using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Event.Commands.DeleteEvent
{
    public record DeleteEventCommand(int EventId) : IRequest<ErrorOr<Unit>>;
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, ErrorOr<Unit>>
    { 
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;        
        private readonly IEventRepository _eventRepository;


        public DeleteEventCommandHandler(           
            ILoggerManager logger,
            IUserRepository userRepository,
            IEventRepository eventRepository)
        {
           
            _logger = logger;
            _userRepository = userRepository;       
            _eventRepository = eventRepository;          

            
        }

        public async Task<ErrorOr<Unit>> Handle(DeleteEventCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

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

            eventFromDb.IsDeleted = true;

            _eventRepository.Update(eventFromDb);

            _logger.LogInfo($"Event with ID: {command.EventId} was deleted");


            return Unit.Value;
        }
    }
}
