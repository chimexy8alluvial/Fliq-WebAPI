using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;

namespace Fliq.Application.Event.Commands.ApproveEvent
{
    public record ApproveEventCommand(int EventId) : IRequest<ErrorOr<Unit>>;
    public class ApproveEventCommandHandler : IRequestHandler<ApproveEventCommand, ErrorOr<Unit>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;

        public ApproveEventCommandHandler(
            ILoggerManager logger,
            IUserRepository userRepository,
            IEventRepository eventRepository)
        {

            _logger = logger;
            _userRepository = userRepository;
            _eventRepository = eventRepository;


        }

        public async Task<ErrorOr<Unit>> Handle(ApproveEventCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Approving event with ID: {command.EventId}");
            var eventDetails = _eventRepository.GetEventById(command.EventId);
            if (eventDetails == null)
            {
                _logger.LogError($"Event with ID: {command.EventId} was not found.");
                return Errors.Event.EventNotFound;
            }

            if (eventDetails.Status != EventStatus.PendingApproval)
            {
                _logger.LogError($"Event with ID: {command.EventId} has been approved already.");
                return Errors.Event.EventApprovedAlready;
            }

            var user = _userRepository.GetUserById(eventDetails.UserId);
            if (user == null)
            {
                _logger.LogError($"User with Id: {eventDetails.UserId} was not found.");
                return Errors.User.UserNotFound;
            }

            eventDetails.Status = EventStatus.Upcoming;

            _eventRepository.Update(eventDetails);

            _logger.LogInfo($"Event with ID: {command.EventId} was approved");

            return Unit.Value;
        }

    }
}