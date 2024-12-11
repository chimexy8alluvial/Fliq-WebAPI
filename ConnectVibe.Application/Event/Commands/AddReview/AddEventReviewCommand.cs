using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.Event;
using MediatR;
using Fliq.Domain.Common.Errors;
using Fliq.Application.Event.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;

namespace Fliq.Application.Event.Commands.AddEventReview
{
    public class AddEventReviewCommand : IRequest<ErrorOr<AddReviewResult>>
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; } // Rating out of 5
        public string Comments { get; set; } = default!;
    }

    public class AddEventReviewCommandHandler : IRequestHandler<AddEventReviewCommand, ErrorOr<AddReviewResult>>
    {
        private readonly IEventReviewRepository _eventReviewRepository;
        private readonly ILoggerManager _logger;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public AddEventReviewCommandHandler(
            IEventReviewRepository eventReviewRepository,
            ILoggerManager logger,IEventRepository eventRepository, IUserRepository userRepository, IMediator mediator)
        {
            _eventReviewRepository = eventReviewRepository;
            _logger = logger;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<AddReviewResult>> Handle(AddEventReviewCommand command, CancellationToken cancellationToken)
        {
            // Retrieve the event
            var eventDetails = _eventRepository.GetEventById(command.EventId);
            if (eventDetails == null)
            {
                _logger.LogError($"Event with ID {command.EventId} not found.");
                return Errors.Event.EventNotFound;
            }

            //Find reviewer
            var reviewer = _userRepository.GetUserById(command.UserId);
            if(reviewer == null)
            {
                _logger.LogError($"User with ID {command.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            // Create new review
            var newReview = new EventReview
            {
                UserId = command.UserId,
                Rating = command.Rating,
                Comments = command.Comments,
                EventId = command.EventId,
            };

            _eventReviewRepository.Add(newReview);

            _logger.LogInfo($"Review added for event ID {command.EventId} by user ID {command.UserId}.");


            // Trigger Notification for Event Organizer
            var organizer = _userRepository.GetUserById(eventDetails.UserId);
            if (organizer != null)
            {
                // Prepare notification details
                var notificationTitle = "New Event Review Submitted";
                var notificationMessage = $"{reviewer.DisplayName} rated your event '{eventDetails.EventTitle}' with {command.Rating} stars. Comment: {command.Comments}";

                // Send Notification
                await _mediator.Publish(new EventReviewSubmittedEvent(
                    reviewer.Id,
                    organizer.Id,
                    eventDetails.Id,
                    command.Rating,
                    command.Comments,
                    notificationTitle,
                    notificationMessage
                ), cancellationToken);
            }

            return new AddReviewResult(newReview);
        }
    }
}