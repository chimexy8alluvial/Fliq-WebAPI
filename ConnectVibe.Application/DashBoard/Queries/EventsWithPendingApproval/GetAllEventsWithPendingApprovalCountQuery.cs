using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.EventsWithPendingApproval
{
    public record GetAllEventsWithPendingApprovalCountQuery() : IRequest<ErrorOr<CountResult>>;
    public class GetAllEventsWithPendingApprovalCountQueryHandler : IRequestHandler<GetAllEventsWithPendingApprovalCountQuery, ErrorOr<CountResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetAllEventsWithPendingApprovalCountQueryHandler(IEventRepository eventRepository, ILoggerManager logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<CountResult>> Handle(GetAllEventsWithPendingApprovalCountQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all events with pending approval count...");

            var count = await _eventRepository.CountAllEventsWithPendingApproval();
            _logger.LogInfo($"All events with pending approval count: {count}");

            return new CountResult(count);
        }
    }
}
