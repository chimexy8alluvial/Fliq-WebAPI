using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Common.Errors;
using MediatR;
using Error = ErrorOr.Error;

namespace Fliq.Application.DashBoard.Queries.OtherTicketCount
{
    public record GetEventOtherTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;
    public class GetEventOtherTicketCountQueryHandler : IRequestHandler<GetEventOtherTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetEventOtherTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger, IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventOtherTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching Other ticket count for EventId: {query.EventId}");

                var eventFromDb = _eventRepository.GetEventById(query.EventId);
                if (eventFromDb == null)
                {
                    _logger.LogError($"Event with ID: {query.EventId} was not found.");
                    return Errors.Event.EventNotFound;
                }


                var count = await _ticketRepository.GetOtherTicketCountAsync(query.EventId);
                _logger.LogInfo($"Other Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching other ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetOtherTicketCountFailed", $"Failed to fetch other ticket count: {ex.Message}");
            }
        }
    }
}