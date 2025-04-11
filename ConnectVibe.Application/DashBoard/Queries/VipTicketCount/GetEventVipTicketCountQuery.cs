using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Common.Errors;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = ErrorOr.Error;

namespace Fliq.Application.DashBoard.Queries.VipTicketCount
{
    public record GetEventVipTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;

    public class GetEventVipTicketCountQueryHandler : IRequestHandler<GetEventVipTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetEventVipTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger, IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventVipTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                 _logger.LogInfo($"Fetching VIP ticket count for EventId: {query.EventId}");

                var eventFromDb = _eventRepository.GetEventById(query.EventId);
                if (eventFromDb == null)
                {
                    _logger.LogError($"Event with ID: {query.EventId} was not found.");
                    return Errors.Event.EventNotFound;
                }


                var count = await _ticketRepository.GetVipTicketCountAsync(query.EventId);
                _logger.LogInfo($"VIP Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching VIP ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetVipTicketCountFailed", $"Failed to fetch VIP ticket count: {ex.Message}");
            }
        }
    }
}

