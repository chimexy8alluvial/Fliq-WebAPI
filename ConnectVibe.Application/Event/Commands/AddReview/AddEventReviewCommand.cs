using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;
using Fliq.Domain.Common.Errors;
using Fliq.Application.Event.Common;

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
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;

        public AddEventReviewCommandHandler(
            IEventReviewRepository eventReviewRepository,
            ILoggerManager logger,
            IMapper mapper, IEventRepository eventRepository)
        {
            _eventReviewRepository = eventReviewRepository;
            _logger = logger;
            _mapper = mapper;
            _eventRepository = eventRepository;
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

            return new AddReviewResult(newReview);
        }
    }
}