using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.EventsCount
{
    public record GetAllEventsCountQuery() : IRequest<ErrorOr<EventCountResult>>;
    public class GetAllEventsCountQueryHandler : IRequestHandler<GetAllEventsCountQuery, ErrorOr<EventCountResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetAllEventsCountQueryHandler(IEventRepository eventRepository, ILoggerManager logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<EventCountResult>> Handle(GetAllEventsCountQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all events count...");

            var count = await _eventRepository.CountAllEvents();
            _logger.LogInfo($"All events count: {count}");

            return new EventCountResult(count);
        }
    }
}
