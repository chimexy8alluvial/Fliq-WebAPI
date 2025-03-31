using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Event.Queries.GetEvent
{
    public record GetEventQuery(int EventId) : IRequest<ErrorOr<CreateEventResult>>;

    public class GetEventQueryHandler : IRequestHandler<GetEventQuery, ErrorOr<CreateEventResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetEventQueryHandler(IEventRepository eventRepository, ILoggerManager logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateEventResult>> Handle(GetEventQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Getting event with id: {query.EventId}");
            var eventEntity = _eventRepository.GetEventById(query.EventId);
            if (eventEntity == null)
            {
                _logger.LogError($"Event with id: {query.EventId} not found");
                return Errors.Event.EventNotFound;
            }

            return new CreateEventResult(eventEntity);
        }
    }
}