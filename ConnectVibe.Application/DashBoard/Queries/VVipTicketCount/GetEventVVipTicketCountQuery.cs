using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.VVipTicketCount
{
    public record GetEventVVipTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;
    public class GetEventVVipTicketCountQueryHandler : IRequestHandler<GetEventVVipTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetEventVVipTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger, IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventVVipTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching VVIP ticket count for EventId: {query.EventId}");

                var eventFromDb = _eventRepository.GetEventById(query.EventId);
                if (eventFromDb == null)
                {
                    _logger.LogError($"Event with ID: {query.EventId} was not found.");
                    return Errors.Event.EventNotFound;
                }

                var count = await _ticketRepository.GetVVipTicketCountAsync(query.EventId);
                _logger.LogInfo($"VVIP Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching VVIP ticket count for EventId {query.EventId}: {ex.Message}");
                return Errors.Ticket.VVipCountFetchFailed(ex.Message);
            }
        }
    }
}